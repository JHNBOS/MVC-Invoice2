using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Controllers {

    public class CompaniesController : Controller {

        //Instances
        private ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context) {
            _context = context;
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all companies
        private async Task<List<Company>> GetCompanies() {
            return await _context.Companies.ToListAsync();
        }

        //Get company based on id
        private async Task<Company> GetCompany(int? id) {
            Company company = null;

            try {
                company = await _context.Companies.SingleOrDefaultAsync(s => s.CompanyID == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return company;
        }

        //Add company to the database
        private async Task CreateCompany(Company company) {
            try {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing company
        private async Task UpdateCompany(Company company) {
            try {
                _context.Update(company);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing company from the database
        private async Task DeleteCompany(int id) {
            Company company = await GetCompany(id);

            try {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //------------------------------------------------------------------------
        //Controller actions

        //GET => Companies/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "Companies";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.CompanyNameSortParm = sortOrder == "CompanyName" ? "companyname_desc" : "CompanyName";
            ViewBag.CityNameSortParm = sortOrder == "City" ? "city_desc" : "City";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

            //Search function
            var companies = await GetCompanies();
            var query = from company in companies
                        select company;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.CompanyName.Contains(searchQuery)
                                       || s.Email.Contains(searchQuery)
                                       || s.City.Contains(searchQuery));
            }

            switch (sortOrder) {
                //Default
                case "begin_desc":
                    query = query.OrderByDescending(s => s.CompanyName);
                    break;

                //Sort on name
                case "CompanyName":
                    query = query.OrderBy(s => s.CompanyName);
                    break;
                case "companyname_desc":
                    query = query.OrderByDescending(s => s.CompanyName);
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
                    query = query.OrderBy(s => s.CompanyName);
                    break;
            }

            return View(query);
        }

        //---------------------------------

        //GET => Companies/Details/5
        public async Task<IActionResult> Details(int? id) {

            //Current page
            ViewBag.Current = "Companies";

            if (id == null) {
                return NotFound();
            }

            //Get company
            var company = await GetCompany(id);

            if (company == null) {
                return NotFound();
            }

            return View(company);
        }

        //---------------------------------

        //GET => Companies/Create
        public IActionResult Create() {

            //Current page
            ViewBag.Current = "Companies";

            return View();
        }

        //---------------------------------

        //POST => Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyID,CompanyName,Email,Phone,BankAccount,Address,PostalCode,City,Country,RegNumber,FinancialNumber,EUFinancialNumber,BankName")] Company company) {

            if (ModelState.IsValid) {
                //Add company to database
                await CreateCompany(company);
                return RedirectToAction("Index");
            }

            return View(company);
        }

        //---------------------------------

        //GET => Companies/Edit/5
        public async Task<IActionResult> Edit(int? id) {

            //Current page
            ViewBag.Current = "Companies";

            if (id == null) {
                return NotFound();
            }

            //Get company
            var company = await GetCompany(id);

            if (company == null) {
                return NotFound();
            }

            return View(company);
        }

        //---------------------------------

        //POST => Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyID,CompanyName,Email,Phone,BankAccount,Address,PostalCode,City,Country,RegNumber,FinancialNumber,EUFinancialNumber,BankName")] Company company) {

            if (id != company.CompanyID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    //Update administrator
                    await UpdateCompany(company);
                } catch (DbUpdateConcurrencyException) {
                    if (!CompanyExists(company.CompanyID)) {
                        return NotFound();
                    } else { throw; }
                }

                return RedirectToAction("Index");
            }

            return View(company);
        }

        //---------------------------------

        //GET => Companies/Delete/5
        public async Task<IActionResult> Delete(int? id) {

            //Current page
            ViewBag.Current = "Companies";

            if (id == null) {
                return NotFound();
            }

            var company = await GetCompany(id);

            if (company == null) {
                return NotFound();
            }

            return View(company);
        }

        //---------------------------------

        //POST => Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            //Delete administrator from database
            await DeleteCompany(id);

            return RedirectToAction("Index");
        }

        //---------------------------------

        private bool CompanyExists(int id) {
            return _context.Companies.Any(e => e.CompanyID == id);
        }

    }
}