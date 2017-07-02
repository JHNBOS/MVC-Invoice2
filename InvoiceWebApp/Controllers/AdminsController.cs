using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Controllers {

    public class AdminsController : Controller {

        //Instances
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;

        public AdminsController(ApplicationDbContext context, IHostingEnvironment env) {
            _context = context;
            _env = env;
            _settings = _context.Settings.SingleOrDefault();
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all administrators
        private async Task<List<Admin>> GetAdmins() {
            return await _context.Admins.ToListAsync();
        }

        //Get administrator based on id
        private async Task<Admin> GetAdmin(int? id) {
            Admin admin = null;

            try {
                admin = await _context.Admins.SingleOrDefaultAsync(s => s.AdminID == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return admin;
        }

        //Add administrator to the database
        private async Task CreateAdmin(Admin admin) {
            try {
                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing administrator
        private async Task UpdateAdmin(Admin admin) {
            try {
                _context.Update(admin);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing administrator from the database
        private async Task DeleteAdmin(int id) {
            Admin admin = await GetAdmin(id);

            try {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //------------------------------------------------------------------------
        //Controller actions

        //GET => Admins/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "Admins";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

            //Search function
            var admins = await GetAdmins();
            var query = from admin in admins
                        select admin;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.FirstName.Contains(searchQuery)
                                       || s.Email.Contains(searchQuery)
                                       || s.LastName.Contains(searchQuery));
            }

            switch (sortOrder) {
                //Default
                case "begin_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;

                //Sort on first name
                case "FirstName":
                    query = query.OrderBy(s => s.FirstName);
                    break;
                case "firstname_desc":
                    query = query.OrderByDescending(s => s.FirstName);
                    break;

                //Sort on email
                case "Email":
                    query = query.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;

                //Sort on last name
                case "LastName":
                    query = query.OrderBy(s => s.LastName);
                    break;
                case "lastname_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;

                //Default
                default:
                    query = query.OrderBy(s => s.LastName);
                    break;
            }

            return View(query);
        }

        //---------------------------------

        //GET => Admins/Details/5
        public async Task<IActionResult> Details(int? id) {

            //Current page
            ViewBag.Current = "Admins";

            if (id == null) {
                return NotFound();
            }

            //Get administrator
            var admin = await GetAdmin(id);

            if (admin == null) {
                return NotFound();
            }

            return View(admin);
        }

        //---------------------------------

        //GET => Admins/Create
        public IActionResult Create() {

            //Current page
            ViewBag.Current = "Admins";

            return View();
        }

        //---------------------------------

        //POST => Admins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminID,FirstName,LastName,Email,Password")] Admin admin) {

            if (ModelState.IsValid) {
                //Add administrator to database
                await CreateAdmin(admin);
                return RedirectToAction("Login", new { area = "" });
            }

            return View(admin);
        }

        //---------------------------------

        //GET => Admins/Edit/5
        public async Task<IActionResult> Edit(int? id) {

            //Current page
            ViewBag.Current = "Manage";

            if (id == null) {
                return NotFound();
            }

            //Get administrator
            var admin = await GetAdmin(id);

            if (admin == null) {
                return NotFound();
            }

            return View(admin);
        }

        //---------------------------------

        //POST => Admins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminID,FirstName,LastName,Email,Password")] Admin admin) {

            if (id != admin.AdminID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    //Update administrator
                    await UpdateAdmin(admin);
                } catch (DbUpdateConcurrencyException) {
                    if (!AdminExists(admin.AdminID)) {
                        return NotFound();
                    } else { throw; }
                }

                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View(admin);
        }

        //---------------------------------

        //GET => Admins/Delete/5
        public async Task<IActionResult> Delete(int? id) {

            //Current page
            ViewBag.Current = "Admins";

            if (id == null) {
                return NotFound();
            }

            //Get administrator
            var admin = await GetAdmin(id);

            if (admin == null) {
                return NotFound();
            }

            return View(admin);
        }

        //---------------------------------

        //POST => Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            //Delete administrator from database
            await DeleteAdmin(id);

            return RedirectToAction("Index");
        }

        //---------------------------------

        //GET => Admins/Login
        public ActionResult Login() {

            //Current page
            ViewBag.Current = "AdminLogin";

            return View();
        }

        //---------------------------------

        //POST => Admins/Login
        [HttpPost]
        public ActionResult Login(Admin admin) {

            Admin login = null;

            try {
                //Get admin based on email and password
                login = _context.Admins.Where(a => a.Email == admin.Email && a.Password == admin.Password)
                    .FirstOrDefault();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            //Set admin session variable
            if (login != null) {
                SessionHelper.Set(this.HttpContext.Session, "Admin", login);
                return RedirectToAction("Index", "Home", new { email = login.Email });
            }

            return View(login);
        }

        //---------------------------------

        //POST => Admins/Logout
        public ActionResult Logout() {

            //Remove session variables and return to sign in view
            HttpContext.Session.Remove("Admin");
            HttpContext.Session.Remove("User");
            return RedirectToAction("Login", "Admins", new { area = "" });
        }

        //---------------------------------

        //GET => Admins/ForgotPassword
        public ActionResult ForgotPassword() {

            //Current page
            ViewBag.Current = "AdminLogin";

            return View();
        }

        //---------------------------------

        //POST => Admins/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email, string password) {

            Admin admin = null;

            try {
                //Get admin based on email
                admin = _context.Admins.SingleOrDefault(m => m.Email == email);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            if (admin == null) {
                return NotFound();
            }

            try {
                //Update administrator with new password
                admin.Password = password;
                _context.Update(admin);
                _context.SaveChanges();

                return RedirectToAction("Login", "Admins", new { area = "" });
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                return View(admin);
            }
        }

        //---------------------------------

        //GET => Admins/Settings
        public ActionResult Settings() {

            //Current page
            ViewBag.Current = "Settings";

            return View(_settings);
        }

        //---------------------------------

        //POST => Admins/Settings
        [HttpPost]
        public async Task<ActionResult> Settings(AppSettings AppSettings, IFormFile file) {

            if (AppSettings != null) {
                try {
                    if (file != null) {
                        var uploads = Path.Combine(_env.WebRootPath, "images");

                        if (file.Length > 0) {
                            var filePath = Path.Combine(uploads, file.FileName);
                            _settings.Logo = (file.FileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                                await file.CopyToAsync(fileStream);
                            }
                        }
                    }
                } catch (Exception ex) {
                    Debug.WriteLine(ex);
                }

                //Update settings with new value(s)
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

                if (AppSettings.Logo != null) {
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

        //---------------------------------

        private bool AdminExists(int id) {
            return _context.Admins.Any(e => e.AdminID == id);
        }

    }
}