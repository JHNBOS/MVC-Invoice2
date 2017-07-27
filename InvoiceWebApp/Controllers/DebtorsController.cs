using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using InvoiceWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceWebApp.Controllers {

    public class DebtorsController : Controller {

        //Instances
        private ApplicationDbContext _context;
        private AppSettings _settings;

        public DebtorsController(ApplicationDbContext context) {
            _context = context;
            _settings = _context.Settings.FirstOrDefault();
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all debtors
        private async Task<List<Debtor>> GetDebtors() {
            return await _context.Debtors.ToListAsync();
        }

        //Get debtor based on id
        private async Task<Debtor> GetDebtor(int? id) {
            Debtor debtor = null;

            try {
                debtor = await _context.Debtors.SingleOrDefaultAsync(s => s.DebtorID == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return debtor;
        }

        //Add debtor to the database
        private async Task CreateDebtor(Debtor debtor) {
            try {
                _context.Debtors.Add(debtor);
                await _context.SaveChangesAsync();

                await CreateLogin(debtor);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Create new login for debtor
        private async Task CreateLogin(Debtor debtor) {
            try {
                User user = new User();
                user.Debtor = debtor;
                user.Email = debtor.Email;
                user.Password = debtor.FirstName + "_" + DateTime.Now.ToString("ddMMHH");

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                //Send email with username and password
                AuthMessageSender email = new AuthMessageSender(_settings);
                await email.SendLoginEmailAsync(user.Email, user.Password);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing debtor
        private async Task UpdateDebtor(Debtor debtor) {
            try {
                _context.Update(debtor);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing debtor and all his invoices and login from the database
        private async Task DeleteDebtor(int id) {
            Debtor debtor = await GetDebtor(id);
            List<Invoice> invoices = null;

            try {
                invoices = _context.Invoices.Where(s => s.DebtorID == debtor.DebtorID).ToList();

                //Remove all invoices and invoice items related to this debtor
                foreach (var invoice in invoices) {
                    _context.InvoiceItems.RemoveRange(_context.InvoiceItems.Where(s => s.InvoiceNumber == invoice.InvoiceNumber));
                    await _context.SaveChangesAsync();
                }

                _context.Invoices.RemoveRange(invoices);
                _context.Users.Remove(_context.Users.Single(s => s.Email == debtor.Email));
                await _context.SaveChangesAsync();

                Thread.Sleep(1500);

                _context.Debtors.Remove(debtor);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //------------------------------------------------------------------------
        //Controller actions

        //GET => Debtors/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "Debtors";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "firstname_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastname_desc" : "LastName";
            ViewBag.CityNameSortParm = sortOrder == "City" ? "city_desc" : "City";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

            //Search function
            var debtors = await GetDebtors();
            var query = from debtor in debtors
                        select debtor;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.FirstName.Contains(searchQuery)
                                       || s.LastName.Contains(searchQuery)
                                       || s.Email.Contains(searchQuery)
                                       || s.City.Contains(searchQuery));
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

                //Sort on last name
                case "LastName":
                    query = query.OrderBy(s => s.LastName);
                    break;
                case "lastname_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;

                //Sort on email
                case "Email":
                    query = query.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;

                //Sort on city
                case "City":
                    query = query.OrderBy(s => s.City);
                    break;
                case "city_desc":
                    query = query.OrderByDescending(s => s.City);
                    break;

                //Default
                default:
                    query = query.OrderBy(s => s.FirstName);
                    break;
            }

            return View(query);
        }

        //---------------------------------

        //GET => Debtors/Details/5
        public async Task<IActionResult> Details(int? id) {

            //Current page
            ViewBag.Current = "Debtors";

            if (id == null) {
                return NotFound();
            }

            //Get debtor
            var debtor = await GetDebtor(id);

            if (debtor == null) {
                return NotFound();
            }

            return View(debtor);
        }

        //---------------------------------

        //GET => Debtors/Create
        public IActionResult Create() {

            //Current page
            ViewBag.Current = "Debtors";

            return View();
        }

        //---------------------------------

        //POST => Debtors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DebtorID,FirstName,LastName,IdNumber,Email,Phone,BankAccount,Address,PostalCode,City,Country")] Debtor debtor) {

            if (ModelState.IsValid) {
                //Add debtor to database
                await CreateDebtor(debtor);
                return RedirectToAction("Index", "Debtors", null);
            }

            return View(debtor);
        }

        //---------------------------------

        //GET => Debtor/Edit/5        
        public async Task<IActionResult> Edit(int? id) {

            //Current page
            ViewBag.Current = "Debtors";

            if (id == null) {
                return NotFound();
            }

            //Get debtor
            var debtor = await GetDebtor(id);

            if (debtor == null) {
                return NotFound();
            }

            return View(debtor);
        }

        //---------------------------------

        //POST => Debtors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DebtorID,FirstName,LastName,IdNumber,Email,Phone,BankAccount,Address,PostalCode,City,Country")] Debtor debtor) {

            if (id != debtor.DebtorID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    //Update debtor
                    await UpdateDebtor(debtor);
                } catch (DbUpdateConcurrencyException) {
                    if (!DebtorExists(debtor.DebtorID)) {
                        return NotFound();
                    } else { throw; }
                }

                return RedirectToAction("Index", "Debtors", null);
            }

            return View(debtor);
        }

        //---------------------------------

        //GET => Debtors/Delete/5
        public async Task<IActionResult> Delete(int? id) {

            //Current page
            ViewBag.Current = "Debtors";

            if (id == null) {
                return NotFound();
            }

            //Get debtor
            var debtor = await GetDebtor(id);

            if (debtor == null) {
                return NotFound();
            }

            return View(debtor);
        }

        //---------------------------------

        //POST => Debtors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            //Delete administrator from database
            await DeleteDebtor(id);

            return RedirectToAction("Index", "Debtors", null);
        }

        //---------------------------------

        private bool DebtorExists(int id) {
            return _context.Debtors.Any(e => e.DebtorID == id);
        }

    }
}