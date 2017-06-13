using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InvoiceWebApp.Models;

namespace InvoiceWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //--------Debtor Unique Keys-------------
            builder.Entity<Debtor>()
            .HasAlternateKey(c => c.IdNumber)
            .HasName("AlternateKey_IdNumber");

            builder.Entity<Debtor>()
            .HasAlternateKey(c => c.BankAccount)
            .HasName("AlternateKey_BankAccount");
            /*-------------------------------------*/
            //--------Product Unique Keys-------------
            builder.Entity<Product>()
            .HasAlternateKey(c => c.Name)
            .HasName("AlternateKey_Name");
            /*-------------------------------------*/
            //--------Company Unique Keys-------------
            builder.Entity<Company>()
            .HasAlternateKey(c => c.RegNumber)
            .HasName("AlternateKey_RegNumber");

            builder.Entity<Company>()
            .HasAlternateKey(c => c.FinancialNumber)
            .HasName("AlternateKey_FinancialNumber");

            builder.Entity<Company>()
            .HasAlternateKey(c => c.EUFinancialNumber)
            .HasName("AlternateKey_EUFinancialNumber");

            builder.Entity<Company>()
            .HasAlternateKey(c => c.BankAccount)
            .HasName("AlternateKey_BankAccount");
            /*-------------------------------------*/
            //--------Admin Unique Keys-------------
            builder.Entity<Admin>()
            .HasAlternateKey(c => c.Email)
            .HasName("AlternateKey_AdminEmail");
            /*-------------------------------------*/
        }

        public DbSet<Debtor> Debtors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<AppSettings> Settings { get; set; }
    }
}
