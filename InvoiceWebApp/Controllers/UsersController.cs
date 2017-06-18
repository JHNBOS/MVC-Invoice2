using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace InvoiceWebApp.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;

        public UsersController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
            _settings = _context.Settings.SingleOrDefault();
        }

        /*----------------------------------------------------------------------*/
        //DATABASE ACTION METHODS

        private async Task<List<User>> GetUsers()
        {
            List<User> userList = await _context.Users.ToListAsync();
            return userList;
        }

        private async Task<User> GetUser(int? id)
        {
            User user = null;

            try
            {
                user = await _context.Users.SingleOrDefaultAsync(s => s.ID == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return user;
        }

        private async Task CreateUser(User user)
        {
            user.AccountType = "Client";

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task UpdateUser(User user)
        {
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task DeleteUser(int id)
        {
            User user = await GetUser(id);

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /*----------------------------------------------------------------------*/
        //CONTROLLER ACTIONS

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await GetUsers());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                await CreateUser(user);
                return RedirectToAction("Login", "Users", new { area = "" });
            }

            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //CURRENT PAGE
            ViewBag.Current = "UserManage";

            if (id == null)
            {
                return NotFound();
            }

            var user = await GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AccountType,Address,City,Country,Email,FirstName,LastName,Password,PostalCode")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await UpdateUser(user);

                User currentUser = SessionHelper.Get<User>(this.HttpContext.Session, "User");
                return RedirectToAction("Index", "Home", new { email = currentUser.Email });
            }

            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await DeleteUser(id);
            return RedirectToAction("Login", "Users", new { area = "" });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

        //GET: User/Login
        public ActionResult Login()
        {
            //CURRENT PAGE
            ViewBag.Current = "Login";

            return View();
        }

        //POST: User/Login
        [HttpPost]
        public ActionResult Login(User user)
        {
            User login = null;
            Debtor debtor = null;

            try
            {
                login = _context.Users.SingleOrDefault(u => u.Email == user.Email && u.Password == user.Password);
                debtor = _context.Debtors.SingleOrDefault(d => d.DebtorID == login.DebtorID);

                login.Debtor = debtor;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (login != null)
            {
                SessionHelper.Set(this.HttpContext.Session, "User", login);
                return RedirectToAction("Index", "Home", new { email = login.Email });
            }

            return View(login);
        }

        //GET: User/Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Login", "Users", new { area = "" });
        }

        //GET: User/ForgotPassword
        public ActionResult ForgotPassword()
        {
            //CURRENT PAGE
            ViewBag.Current = "Login";

            return View();
        }

        //POST: User/ForgotPassword
        [HttpPost]
        public ActionResult ForgotPassword(string email, string password)
        {
            User user = null;

            try
            {
                user = _context.Users.SingleOrDefault(m => m.Email == email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.Password = password;
                _context.Update(user);
                _context.SaveChanges();

                return RedirectToAction("Login", "Users", new { area = "" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return View(user);
            }
        }
        

    }
}