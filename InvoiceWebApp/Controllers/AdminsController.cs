using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace InvoiceWebApp.Controllers
{
    public class AdminsController : Controller
    {
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;

        public AdminsController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
            _settings = _context.Settings.SingleOrDefault();
        }

        /*----------------------------------------------------------------------*/
        //DATABASE ACTION METHODS

        private async Task<List<Admin>> GetAdmins()
        {
            List<Admin> adminList = await _context.Admins.ToListAsync();
            return adminList;
        }

        private async Task<Admin> GetAdmin(int? id)
        {
            Admin admin = null;

            try
            {
                admin = await _context.Admins.SingleOrDefaultAsync(s => s.AdminID == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return admin;
        }

        private async Task CreateAdmin(Admin admin)
        {
            try
            {
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task UpdateAdmin(Admin admin)
        {
            try
            {
                _context.Update(admin);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task DeleteAdmin(int id)
        {
            Admin admin = await GetAdmin(id);

            try
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /*----------------------------------------------------------------------*/
        //CONTROLLER ACTIONS

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            //CURRENT PAGE
            ViewBag.Current = "Admins";

            return View(await _context.Admins.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //CURRENT PAGE
            ViewBag.Current = "Admins";

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .SingleOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            //CURRENT PAGE
            ViewBag.Current = "Admins";

            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminID,FirstName,LastName,Email,Password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                await CreateAdmin(admin);
                return RedirectToAction("Login", new { area = "" });
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //CURRENT PAGE
            ViewBag.Current = "Manage";

            if (id == null)
            {
                return NotFound();
            }

            var admin = await GetAdmin(id);

            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminID,FirstName,LastName,Email,Password")] Admin admin)
        {
            if (id != admin.AdminID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await UpdateAdmin(admin);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home",  new { area = "" });
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //CURRENT PAGE
            ViewBag.Current = "Admins";

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .SingleOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(m => m.AdminID == id);
            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //GET: Admins/Login
        public ActionResult Login()
        {
            //CURRENT PAGE
            ViewBag.Current = "AdminLogin";

            return View();
        }

        //POST: User/Login
        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            Admin login = null;

            try
            {
                login = _context.Admins.Where(a => a.Email == admin.Email && a.Password == admin.Password).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (login != null)
            {
                SessionHelper.Set(this.HttpContext.Session, "Admin", login);
                return RedirectToAction("Index", "Home", new { email = login.Email });
            }

            return View(login);
        }

        //GET: Admins/Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Login", "Admins", new { area = "" });
        }

        //GET: Admins/ForgotPassword
        public ActionResult ForgotPassword()
        {
            //CURRENT PAGE
            ViewBag.Current = "AdminLogin";

            return View();
        }

        //POST: Admins/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email, string password)
        {
            Admin admin = null;

            try
            {
                admin = _context.Admins.SingleOrDefault(m => m.Email == email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (admin == null)
            {
                return NotFound();
            }

            try
            {
                admin.Password = password;
                _context.Update(admin);
                _context.SaveChanges();

                return RedirectToAction("Login", "Admins", new { area = "" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return View(admin);
            }
        }

        //GET: User/Settings
        public ActionResult Settings()
        {
            //CURRENT PAGE
            ViewBag.Current = "Settings";

            return View(_settings);
        }

        //POST: User/Settings
        [HttpPost]
        public async Task<ActionResult> Settings(AppSettings AppSettings, IFormFile file)
        {
            if (AppSettings != null)
            {
                try
                {
                    if (file != null)
                    {
                        var uploads = Path.Combine(_env.WebRootPath, "images");

                        if (file.Length > 0)
                        {
                            var filePath = Path.Combine(uploads, file.FileName);
                            _settings.Logo = (file.FileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                _settings.Address = AppSettings.Address;
                _settings.PostalCode = AppSettings.PostalCode;
                _settings.City = AppSettings.City;
                _settings.Country = AppSettings.Country;

                _settings.CompanyName = AppSettings.CompanyName;
                _settings.RegNumber = AppSettings.RegNumber;
                _settings.FinancialNumber = AppSettings.FinancialNumber;
                _settings.EUFinancialNumber = AppSettings.EUFinancialNumber;
                _settings.BankName = AppSettings.BankName;
                _settings.Prefix = AppSettings.Prefix;

                if (AppSettings.Logo != null)
                {
                    _settings.Logo = AppSettings.Logo;
                }

                _settings.UseLogo = AppSettings.UseLogo;

                _settings.Website = AppSettings.Website;
                _settings.Phone = AppSettings.Phone;

                _settings.Email = AppSettings.Email;
                _settings.Password = AppSettings.Password;
                _settings.SMTP = AppSettings.SMTP;
                _settings.Port = AppSettings.Port;

                //Update Settings
                _context.Update(_settings);
                _context.SaveChanges();

                //Remove session variable
                SessionHelper.Set(this.HttpContext.Session, "Settings", null);
                _settings = null;

                //Set new session variable
                _settings = _context.Settings.FirstOrDefault();
                SessionHelper.Set(this.HttpContext.Session, "Settings", _settings);

                Admin currentAdmin = SessionHelper.Get<Admin>(this.HttpContext.Session, "Admin");
                return RedirectToAction("Index", "Home", new { email = currentAdmin.Email });
            }

            return View();
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminID == id);
        }
    }
}
