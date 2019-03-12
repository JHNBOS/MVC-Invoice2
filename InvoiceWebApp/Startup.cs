using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using InvoiceWebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvoiceWebApp
{

	public class Startup
	{
		private string _contentRootPath = "";

		public Startup(IHostingEnvironment env)
		{
			_contentRootPath = env.ContentRootPath;

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			if (env.IsDevelopment())
			{
				// For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets<Startup>();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();

		}

		public IConfigurationRoot Configuration { get; }


		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//Add connection services
			string conn = Configuration.GetConnectionString("DefaultConnection");
			if (conn.Contains("%CONTENTROOTPATH%"))
			{
				conn = conn.Replace("%CONTENTROOTPATH%", _contentRootPath);
			}
			services.AddOptions();

			// Add framework services.
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(conn));

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddDistributedMemoryCache();
			services.AddSession();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			// Add application services.
			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();

			// Add Applciation Services
			services.AddScoped<IViewRenderService, ViewRenderService>();

			services.AddMvc()
				.AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning disable CS0618 // Type or member is obsolete
			loggerFactory.AddDebug();
#pragma warning restore CS0618 // Type or member is obsolete

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseBrowserLink();
			} else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			app.UseSession();

			//Get Session
			RequestContextManager.Instance = new RequestContextManager(app.ApplicationServices.GetService<IHttpContextAccessor>());

#pragma warning disable CS0618 // Type or member is obsolete
			app.UseIdentity();
#pragma warning restore CS0618 // Type or member is obsolete

			app.UseRequestLocalization(new RequestLocalizationOptions {
				DefaultRequestCulture = new RequestCulture("nl-NL")
			});

			// Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				routes.MapRoute(
					name: "Admins",
					template: "{controller=Admins}/{action=Login}/{id?}");
			});
		}
	}
}