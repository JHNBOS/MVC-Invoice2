using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Controllers {

    public class CompaniesController : Controller {
        private ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context) {
            _context = context;
        }

        /*----------------------------------------------------------------------*/
        //DATABASE ACTION METHODS

        private async Task<List<Company>> GetCompanies() {
            List<Company> companyList = await _context.Company.ToListAsync();
            return companyList;
        }

        /*----------------------------------------------------------------------*/
        //CONTROLLER METHODS

        // GET: Companies
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {
            //CURRENT PAGE
            ViewBag.Current = "Companies";

            //SORTING OPTIONS DEBTOR LIST
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.CompanyNameSortParm = sortOrder == "CompanyName" ? "companyname_desc" : "CompanyName";
            ViewBag.CityNameSortParm = sortOrder == "City" ? "city_desc" : "City";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

            var companies = await GetCompanies();
            var query = from company in companies
                        select company;

            //SEARCH OPTION PRODUCT LIST
            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.CompanyName.Contains(searchQuery)
                                       || s.Email.Contains(searchQuery)
                                       || s.City.Contains(searchQuery));
            }

            switch (sortOrder) {
                //WHEN NO SORT
                case "begin_desc":
                    query = query.OrderByDescending(s => s.CompanyName);
                    break;
                //COMPANY NAME
                case "CompanyName":
                    query = query.OrderBy(s => s.CompanyName);
                    break;

                case "companyname_desc":
                    query = query.OrderByDescending(s => s.CompanyName);
                    break;
                //EMAIL
                case "Email":
                    query = query.OrderBy(s => s.Email);
                    break;

                case "email_desc":
                    query = query.OrderByDescending(s => s.Email);
                    break;
                //CITY
                case "City":
                    query = query.OrderBy(s => s.City);
                    break;

                case "city_desc":
                    query = query.OrderByDescending(s => s.City);
                    break;
                //DEFAUlT
                default:
                    query = query.OrderBy(s => s.CompanyName);
                    break;
            }

            return View(query);
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id) {
            //CURRENT PAGE
            ViewBag.Current = "Companies";

            if (id == null) {
                return NotFound();
            }

            var company = await _context.Company
                .SingleOrDefaultAsync(m => m.CompanyID == id);
            if (company == null) {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create() {
            //CURRENT PAGE
            ViewBag.Current = "Companies";

            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyID,CompanyName,Email,Phone,BankAccount,Address,PostalCode,City,Country,RegNumber,FinancialNumber,EUFinancialNumber,BankName")] Company company) {
            if (ModelState.IsValid) {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var company = await _context.Company.SingleOrDefaultAsync(m => m.CompanyID == id);
            if (company == null) {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyID,CompanyName,Email,Phone,BankAccount,Address,PostalCode,City,Country,RegNumber,FinancialNumber,EUFinancialNumber,BankName")] Company company) {
            if (id != company.CompanyID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!CompanyExists(company.CompanyID)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            //CURRENT PAGE
            ViewBag.Current = "Companies";

            if (id == null) {
                return NotFound();
            }

            var company = await _context.Company
                .SingleOrDefaultAsync(m => m.CompanyID == id);
            if (company == null) {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var company = await _context.Company.SingleOrDefaultAsync(m => m.CompanyID == id);
            _context.Company.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CompanyExists(int id) {
            return _context.Company.Any(e => e.CompanyID == id);
        }
    }
}