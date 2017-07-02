using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Controllers {

    public class UsersController : Controller {

        //Instances
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;

        public UsersController(ApplicationDbContext context, IHostingEnvironment env) {
            _context = context;
            _env = env;
            _settings = _context.Settings.SingleOrDefault();
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all users
        private async Task<List<User>> GetUsers() {
            return await _context.Users.Include(s => s.Debtor).ToListAsync();
        }

        //Get user based on id
        private async Task<User> GetUser(int? id) {
            User user = null;

            try {
                user = await _context.Users.Include(s => s.Debtor).SingleOrDefaultAsync(s => s.ID == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return user;
        }

        //Add user to the database
        private async Task CreateUser(User user) {
            //Set account type
            user.AccountType = "Client";

            try {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing user
        private async Task UpdateUser(User user) {
            try {
                _context.Update(user);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing user from the database
        private async Task DeleteUser(int id) {
            User user = await GetUser(id);

            try {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //------------------------------------------------------------------------
        //Controller actions

        //GET => Users/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "Users";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

            //Search function
            var users = await GetUsers();
            var query = from user in users
                        select user;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.Debtor.FirstName.Contains(searchQuery)
                                       || s.Email.Contains(searchQuery)
                                       || s.Debtor.LastName.Contains(searchQuery)
                                       || s.Debtor.City.Contains(searchQuery)
                                       || s.Debtor.PostalCode.Contains(searchQuery)
                                       || s.Debtor.Country.Contains(searchQuery));
            }

            switch (sortOrder) {
                //Default
                case "begin_desc":
                    query = query.OrderByDescending(s => s.Debtor.LastName);
                    break;

                //Sort on first name
                case "FirstName":
                    query = query.OrderBy(s => s.Debtor.FirstName);
                    break;
                case "firstname_desc":
                    query = query.OrderByDescending(s => s.Debtor.FirstName);
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
                    query = query.OrderBy(s => s.Debtor.LastName);
                    break;
                case "lastname_desc":
                    query = query.OrderByDescending(s => s.Debtor.LastName);
                    break;

                //Default
                default:
                    query = query.OrderBy(s => s.Debtor.LastName);
                    break;
            }

            return View(query);
        }

        //---------------------------------

        //GET => Users/Details/5
        public async Task<IActionResult> Details(int? id) {

            //Current page
            ViewBag.Current = "Users";

            if (id == null) {
                return NotFound();
            }

            //Get user
            var user = await GetUser(id);

            if (user == null) {
                return NotFound();
            }

            return View(user);
        }

        //---------------------------------

        //GET => Users/Create
        public IActionResult Create() {

            //Current page
            ViewBag.Current = "Users";

            return View();
        }

        //---------------------------------

        //POST => Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Email,Password")] User user) {

            if (ModelState.IsValid) {
                //Add user to database
                await CreateUser(user);
                return RedirectToAction("Login", "Users", new { area = "" });
            }

            return View(user);
        }

        //---------------------------------

        //GET => Users/Edit/5
        public async Task<IActionResult> Edit(int? id) {

            //Current page
            ViewBag.Current = "Users";

            if (id == null) {
                return NotFound();
            }

            //Get user
            var user = await GetUser(id);

            if (user == null) {
                return NotFound();
            }

            return View(user);
        }

        //---------------------------------

        //POST => Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AccountType,Address,City,Country,Email,FirstName,LastName,Password,PostalCode")] User user) {

            if (id != user.ID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                await UpdateUser(user);

                try {
                    //Update user
                    await UpdateUser(user);
                } catch (DbUpdateConcurrencyException) {
                    if (!UserExists(user.ID)) {
                        return NotFound();
                    } else { throw; }
                }

                User currentUser = SessionHelper.Get<User>(this.HttpContext.Session, "User");
                return RedirectToAction("Index", "Home", new { email = currentUser.Email });
            }

            return View(user);
        }

        //---------------------------------

        //GET => Users/Delete/5
        public async Task<IActionResult> Delete(int? id) {

            //Current page
            ViewBag.Current = "Users";

            if (id == null) {
                return NotFound();
            }

            //Get user
            var user = await GetUser(id);

            if (user == null) {
                return NotFound();
            }

            return View(user);
        }

        //---------------------------------

        //POST => Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            //Delete user from database
            await DeleteUser(id);
            return RedirectToAction("Login", "Users", new { area = "" });
        }

        //---------------------------------

        //GET => Users/Login
        public ActionResult Login() {

            //Current page
            ViewBag.Current = "Login";

            return View();
        }

        //---------------------------------

        //POST => Users/Login
        [HttpPost]
        public ActionResult Login(User user) {
            User userLogin = null;
            Admin adminLogin = null;
            Debtor debtor = null;

            try {
                //Find user based on email and password
                userLogin = _context.Users.SingleOrDefault(u => u.Email == user.Email && u.Password == user.Password);

                if (userLogin != null) {
                    debtor = _context.Debtors.SingleOrDefault(d => d.DebtorID == userLogin.DebtorID);
                    userLogin.Debtor = debtor;
                } else {
                    adminLogin = _context.Admins.SingleOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            //Set session variables
            if (userLogin != null) {
                SessionHelper.Set(this.HttpContext.Session, "User", userLogin);
                return RedirectToAction("Index", "Home", new { email = userLogin.Email });
            }
            if (adminLogin != null) {
                SessionHelper.Set(this.HttpContext.Session, "Admin", adminLogin);
                return RedirectToAction("Index", "Home", new { email = adminLogin.Email });
            }

            return View(userLogin);
        }

        //---------------------------------

        //POST => Users/Logout
        public ActionResult Logout() {

            //Remove session variables and return to login page
            HttpContext.Session.Remove("User");
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Login", "Users", new { area = "" });
        }

        //---------------------------------

        //GET => Users/ForgotPassword
        public ActionResult ForgotPassword() {

            //Current page
            ViewBag.Current = "Login";

            return View();
        }

        //---------------------------------

        //POST => Users/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email, string password) {
            User userLogin = null;
            Admin adminLogin = null;

            try {
                //Find user based on email
                userLogin = _context.Users.SingleOrDefault(s => s.Email == email);

                if (userLogin == null) {
                    adminLogin = _context.Admins.SingleOrDefault(s => s.Email == email);
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            if (userLogin == null && adminLogin == null) {
                return NotFound();
            }

            try {
                //Update password
                if (userLogin != null) {
                    userLogin.Password = password;
                    _context.Update(userLogin);

                } else if (adminLogin != null) {
                    adminLogin.Password = password;
                    _context.Update(adminLogin);
                }

                _context.SaveChanges();

                return RedirectToAction("Login", "Users", new { area = "" });
            } catch (Exception ex) {
                Debug.WriteLine(ex);
                return View(userLogin);
            }
        }

        //---------------------------------

        private bool UserExists(int id) {
            return _context.Users.Any(e => e.ID == id);
        }

    }
}