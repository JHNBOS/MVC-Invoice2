using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using InvoiceWebApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Controllers {

    public class InvoicesController : Controller {

        //Instances
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;

        public InvoicesController(ApplicationDbContext context, IHostingEnvironment env) {
            _context = context;
            _settings = _context.Settings.FirstOrDefault();
            _env = env;
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all invoices
        private async Task<List<Invoice>> GetInvoices() {
            return await _context.Invoices
                        .Include(s => s.Debtor)
                        .Include(s => s.Company)
                        .Include(s => s.InvoiceItems)
                        .ThenInclude(s => s.Product)
                        .ToListAsync();
        }

        //Get invoice based on invoice number
        private async Task<Invoice> GetInvoice(int? id) {
            Invoice invoice = null;

            try {
                invoice = await _context.Invoices
                        .Include(s => s.Debtor)
                        .Include(s => s.Company)
                        .Include(s => s.InvoiceItems)
                        .ThenInclude(s => s.Product)
                        .SingleOrDefaultAsync(s => s.InvoiceNumber == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return invoice;
        }

        //Get a list of all invoices items based on invoice number
        private async Task<List<InvoiceItem>> GetInvoiceItems(int? id) {
            List<InvoiceItem> itemList = null;

            try {
                itemList = await _context.InvoiceItems
                            .Include(d => d.Product)
                            .Where(s => s.InvoiceNumber == id)
                            .ToListAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return itemList;
        }

        //Add invoice and invoice items to the database
        private async Task CreateInvoice(Invoice invoice, string pids, string amounts, string total) {

            //Check if one of these is null or empty
            if (invoice.DebtorID == -1 || invoice.DebtorID == null
                || String.IsNullOrEmpty(invoice.DebtorID.ToString())) {
                invoice.DebtorID = null;
            } else if (invoice.CompanyID == -1 || invoice.CompanyID == null
                || String.IsNullOrEmpty(invoice.CompanyID.ToString())) {
                invoice.CompanyID = null;
            }

            //Variables
            string[] pidArray = null;
            string[] amountArray = null;
            List<InvoiceItem> items = new List<InvoiceItem>();

            try {
                invoice.Total = decimal.Parse(total);
                invoice.Paid = false;

                //Save invoice to database
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                //Check is products were added
                if (pids.Contains(',')) {
                    pidArray = pids.Split(',');
                }

                if (amounts.Contains(',')) {
                    amountArray = amounts.Split(',');
                }

                if (pidArray != null) {
                    //Add all items to the database
                    for (int i = 0; i < pidArray.Length; i++) {
                        int pid = int.Parse(pidArray[i]);
                        int amount = int.Parse(amountArray[i]);

                        InvoiceItem item = new InvoiceItem();
                        item.Amount = amount;
                        item.InvoiceNumber = invoice.InvoiceNumber;
                        item.ProductID = pid;

                        items.Add(item);
                    }

                    _context.AddRange(items);
                } else {
                    //Add only one item to the database
                    InvoiceItem item = new InvoiceItem();
                    item.Amount = int.Parse(amounts);
                    item.InvoiceNumber = invoice.InvoiceNumber;
                    item.ProductID = int.Parse(pids.Split('_')[0]);

                    _context.InvoiceItems.Add(item);
                }

                await _context.SaveChangesAsync();

            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing invoice
        private async Task UpdateInvoice(Invoice invoice, string pids, string amounts, string total) {

            //Variables
            bool isEmpty = false;
            string[] pidArray = null;
            string[] amountArray = null;
            List<InvoiceItem> items = new List<InvoiceItem>();

            try {
                if (pids == "" || pids == null) {
                    isEmpty = true;
                }

                if (isEmpty == false) {
                    Invoice invoiceBeforeUpdate = _context.Invoices
                                .Include(s => s.Debtor)
                                .Include(s => s.Company)
                                .Include(s => s.InvoiceItems)
                                .Single(s => s.InvoiceNumber == invoice.InvoiceNumber);

                    //Update invoice with new values
                    invoiceBeforeUpdate.InvoiceNumber = invoice.InvoiceNumber;
                    invoiceBeforeUpdate.CreatedOn = invoice.CreatedOn;
                    invoiceBeforeUpdate.ExpirationDate = invoice.ExpirationDate;
                    invoiceBeforeUpdate.Type = invoice.Type;
                    invoiceBeforeUpdate.Total = decimal.Parse(total);

                    _context.Update(invoiceBeforeUpdate);
                    _context.InvoiceItems.RemoveRange(_context.InvoiceItems.Where(s => s.InvoiceNumber == invoice.InvoiceNumber));
                    _context.SaveChanges();

                    if (pids.Length > 1) {
                        pidArray = pids.Split(',');
                    }
                    if (amounts.Length > 1) {
                        amountArray = amounts.Split(',');
                    }

                    if (pidArray != null) {
                        for (int i = 0; i < pidArray.Length; i++) {
                            int _pid = int.Parse(pidArray[i]);
                            int _amount = int.Parse(amountArray[i]);

                            InvoiceItem item = new InvoiceItem();
                            item.Amount = _amount;
                            item.InvoiceNumber = invoice.InvoiceNumber;
                            item.ProductID = _pid;

                            items.Add(item);
                        }

                        _context.InvoiceItems.AddRange(items);
                        await _context.SaveChangesAsync();

                    } else if (pidArray == null && (pids != "" || pids != null)) {
                        InvoiceItem item = new InvoiceItem();
                        item.Amount = int.Parse(amounts);
                        item.InvoiceNumber = invoice.InvoiceNumber;
                        item.ProductID = int.Parse(pids.Split('_')[0]);

                        _context.InvoiceItems.Add(item);
                        await _context.SaveChangesAsync();
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing invoice and all it's items from the database
        private async Task DeleteInvoice(int id) {
            Invoice invoice = await GetInvoice(id);
            List<InvoiceItem> itemList = await GetInvoiceItems(id);

            try {
                _context.InvoiceItems.RemoveRange(itemList);
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        public JsonResult GetProducts(int id)
        {
            // Create selectlist with all products
            var products = _context.Products
                .Where(s => s.CategoryID == id)
                .Select(s => new SelectListItem {
                    Value = s.ProductID.ToString() + "_" + s.Price.ToString(),
                    Text = s.Name
                });

            return Json(new SelectList(products, "Value", "Text"));
        }

        public JsonResult GetCategories()
        {
            // Create selectlist with all products
            var categories = _context.Categories
                .Select(s => new SelectListItem {
                    Value = s.CategoryID.ToString(),
                    Text = s.CategoryName
                });

            return Json(new SelectList(categories, "Value", "Text"));
        }

        //------------------------------------------------------------------------
        //Controller actions

        //GET => Invoices/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "Invoices";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.TotalSortParm = sortOrder == "Total" ? "total_desc" : "Total";
            ViewBag.DebtorSortParm = sortOrder == "Debtor" ? "debtor_desc" : "Debtor";

            //Search function
            var invoices = await GetInvoices();
            var query = from invoice in invoices
                        select invoice;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.Debtor.FullName.Contains(searchQuery)
                                    || s.InvoiceNumber.ToString().Contains(searchQuery)
                                    || s.Total.ToString().Contains(searchQuery));
            }

            switch (sortOrder) {
                //Default
                case "begin_desc":
                    query = query.OrderBy(s => s.InvoiceNumber);
                    break;

                //Sort on invoice number
                case "Number":
                    query = query.OrderBy(s => s.InvoiceNumber);
                    break;
                case "number_desc":
                    query = query.OrderByDescending(s => s.InvoiceNumber);
                    break;

                //Sort on debtor
                case "Debtor":
                    query = query.OrderBy(s => s.Debtor.FullName);
                    break;
                case "debtor_desc":
                    query = query.OrderByDescending(s => s.Debtor.FullName);
                    break;

                //Sort on total amount
                case "Total":
                    query = query.OrderBy(s => s.Total);
                    break;
                case "total_desc":
                    query = query.OrderByDescending(s => s.Total);
                    break;

                //Default
                default:
                    query = query.OrderBy(s => s.InvoiceNumber);
                    break;
            }

            return View(query);
        }

        //---------------------------------

        //GET => Invoices/Details/5
        public async Task<IActionResult> Details(int? id) {

            //Current page
            ViewBag.Current = "Invoices";

            if (id == null) {
                return NotFound();
            }

            //Get invoice, invoice items and related products
            Invoice invoice = await GetInvoice(id);
            List<InvoiceItem> invoiceItems = await GetInvoiceItems(id);
            List<Product> productList = new List<Product>();

            if (invoice == null) {
                return NotFound();
            }

            foreach (var item in invoiceItems) {
                Product product = _context.Products.SingleOrDefault(s => s.ProductID == item.ProductID);
                productList.Add(product);
            }

            int cnt = 0;
            string[] pids = new string[invoiceItems.Count];

            foreach (var product in productList) {
                string _id = product.ProductID + "_" + product.Price;
                pids[cnt] = _id;
                cnt++;
            }

            //Viewbags and viewdata
            ViewBag.PIDs = pids;
            ViewBag.Amounts = invoiceItems.Select(s => s.Amount).ToArray();
            ViewBag.Names = invoiceItems.Select(s => s.Product.Name).ToArray();
            ViewBag.Total = String.Format("{0:N2}", invoice.Total);
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyName", invoice.CompanyID);
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "FullName", invoice.DebtorID);

            if (invoice == null) {
                return NotFound();
            }

            return View(invoice);
        }

        //---------------------------------

        //GET => Invoices/Create
        public IActionResult Create() {

            //Current page
            ViewBag.Current = "Invoices";

            //Create selectlist with all categories
            var categories = _context.Categories
                .Select(s => new SelectListItem {
                    Value = s.CategoryID.ToString(),
                    Text = s.CategoryName
            });

            //Create selectlist with all companies
            var companies = _context.Companies
                .Select(s => new SelectListItem {
                    Value = s.CompanyID.ToString(),
                    Text = s.CompanyName.ToString() + " in " + s.City.ToString()
            });

            //Create selectlist with all debtor
            var debtors = _context.Debtors
                .Select(s => new SelectListItem {
                    Value = s.DebtorID.ToString(),
                    Text = s.FullName.ToString() + " in " + s.City.ToString()
            });

            //Viewbags and viewdata
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewData["CompanyID"] = new SelectList(companies, "Value", "Text");
            ViewData["DebtorID"] = new SelectList(debtors, "Value", "Text");

            return View();
        }

        //---------------------------------

        //POST => Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("InvoiceNumber,CreatedOn,DebtorID,CompanyID,Paid,ExpirationDate,Type,Discount")] Invoice invoice,
            string total, string pids, string amounts) {

            if (ModelState.IsValid) {
                //Check if debtor or company id is null
                if (invoice.DebtorID is int) {
                    invoice.CompanyID = null;
                } else if (invoice.CompanyID is int) {
                    invoice.DebtorID = null;
                }

                //Add invoice to database
                await CreateInvoice(invoice, pids, amounts, total);

                //Send email to debtor about new invoice
                if (invoice.Type == "Final") {
                    if (invoice.DebtorID is int) {
                        Debtor debtor = _context.Debtors.Single(s => s.DebtorID == invoice.DebtorID);
                        AuthMessageSender email = new AuthMessageSender(_settings);
                        await email.SendInvoiceEmailAsync(debtor.Email);
                    } else if (invoice.CompanyID is int) {
                        Company company = _context.Companies.Single(s => s.CompanyID == invoice.CompanyID);
                        AuthMessageSender email = new AuthMessageSender(_settings);
                        await email.SendInvoiceEmailAsync(company.Email);
                    }
                }

                return RedirectToAction("Index");
            }

            //Create selectlist with all categories
            var categories = _context.Categories
                .Select(s => new SelectListItem {
                    Value = s.CategoryID.ToString(),
                    Text = s.CategoryName
            });

            //Create selectlist with all companies
            var companies = _context.Companies
                .Select(s => new SelectListItem {
                    Value = s.CompanyID.ToString(),
                    Text = s.CompanyName.ToString() + " in " + s.City.ToString()
            });

            //Create selectlist with all debtor
            var debtors = _context.Debtors
                .Select(s => new SelectListItem {
                    Value = s.DebtorID.ToString(),
                    Text = s.FullName.ToString() + " in " + s.City.ToString()
            });

            //Viewbags and viewdata
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewData["CompanyID"] = new SelectList(companies, "Value", "Text", invoice.CompanyID);
            ViewData["DebtorID"] = new SelectList(debtors, "Value", "Text", invoice.DebtorID);

            return View(invoice);
        }

        //---------------------------------

        //GET => Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id) {

            //Current page
            ViewBag.Current = "Invoices";

            if (id == null) {
                return NotFound();
            }

            //Get invoice and invoice items
            var invoice = await GetInvoice(id);
            var items = await GetInvoiceItems(id);

            if (invoice == null) {
                return NotFound();
            }

            //Get product id's and create unique id
            var p = _context.Products;
            string[] pids = new string[p.Count()];
            int[] cats = new int[p.Count()];

            int cnt = 0;
            foreach (var pid in p) {
                string _id = pid.ProductID + "_" + pid.Price;
                pids[cnt] = _id;
                cats[cnt] = (int)pid.CategoryID;
                cnt++;
            }

            //Create selectlist with all products
            var products = _context.Products
                 .Select(s => new SelectListItem {
                     Value = s.ProductID.ToString() + "_" + s.Price.ToString(),
                     Text = s.Name
                 });

            //Create selectlist with all categories
            var categories = _context.Categories
                .Select(s => new SelectListItem {
                    Value = s.CategoryID.ToString(),
                    Text = s.CategoryName
                });

            //Create selectlist with all companies
            var companies = _context.Companies
                .Select(s => new SelectListItem {
                    Value = s.CompanyID.ToString(),
                    Text = s.CompanyName.ToString() + " in " + s.City.ToString()
                });

            //Create selectlist with all debtor
            var debtors = _context.Debtors
                .Select(s => new SelectListItem {
                    Value = s.DebtorID.ToString(),
                    Text = s.FullName.ToString() + " in " + s.City.ToString()
                });

            //Viewbags and viewdata
            ViewBag.PIDs = pids;
            ViewBag.Cats = cats;
            ViewBag.Amounts = items.Select(s => s.Amount).ToArray();
            ViewBag.Names = items.Select(s => s.Product.Name).ToArray();
            ViewBag.Total = String.Format("{0:N2}", invoice.Total);

            ViewBag.Products = new SelectList(products, "Value", "Text");
            ViewBag.Categories = new SelectList(categories, "Value", "Text");
            ViewData["CompanyID"] = new SelectList(companies, "Value", "Text", invoice.CompanyID);
            ViewData["DebtorID"] = new SelectList(debtors, "Value", "Text", invoice.DebtorID);

            return View(invoice);
        }

        //---------------------------------

        //POST => Invoices/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("InvoiceNumber,CreatedOn,DebtorID,CompanyID,Paid,ExpirationDate,Type,Discount")] Invoice invoice,
            string pids, string amounts, string total) {

            if (id != invoice.InvoiceNumber) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                //Update invoice
                Invoice invoiceBeforeUpdate = _context.Invoices.Single(s => s.InvoiceNumber == invoice.InvoiceNumber);
                await UpdateInvoice(invoice, pids, amounts, total);

                //Send email to debtor about new invoice
                if (invoice.Type == "Final" && invoiceBeforeUpdate.Type != "Final" && !String.IsNullOrEmpty(invoice.DebtorID.ToString())) {
                    Debtor debtor = _context.Debtors.Single(s => s.DebtorID == invoice.DebtorID);
                    AuthMessageSender email = new AuthMessageSender(_settings);
                    await email.SendInvoiceEmailAsync(debtor.Email);

                } else if (invoice.Type == "Final" && invoiceBeforeUpdate.Type != "Final" && String.IsNullOrEmpty(invoice.CompanyID.ToString())) {
                    Company company = _context.Companies.Single(s => s.CompanyID == invoice.CompanyID);
                    AuthMessageSender email = new AuthMessageSender(_settings);
                    await email.SendInvoiceEmailAsync(company.Email);
                }

                return RedirectToAction("Index");
            }

            //Create selectlist with all companies
            var companies = _context.Companies
                .Select(s => new SelectListItem {
                    Value = s.CompanyID.ToString(),
                    Text = s.CompanyName.ToString() + " in " + s.City.ToString()
                });

            //Viewbags and viewdata
            ViewData["CompanyID"] = new SelectList(companies, "Value", "Text", invoice.CompanyID);
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "FullName", invoice.DebtorID);

            return View(invoice);
        }

        //---------------------------------

        //GET => Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id) {

            //Current page
            ViewBag.Current = "Invoices";

            if (id == null) {
                return NotFound();
            }

            //Get invoice and invoice items
            Invoice invoice = await GetInvoice(id);
            List<InvoiceItem> invoiceItems = await GetInvoiceItems(id);
            List<Product> productList = new List<Product>();

            if (invoice == null) {
                return NotFound();
            }

            //Get product based on id and add these to a list
            foreach (var item in invoiceItems) {
                Product product = _context.Products.SingleOrDefault(s => s.ProductID == item.ProductID);
                productList.Add(product);
            }

            //Create selectlist with all products
            int cnt = 0;
            string[] pids = new string[invoiceItems.Count];

            foreach (var product in productList) {
                string _id = product.ProductID + "_" + product.Price;
                pids[cnt] = _id;
                cnt++;
            }

            //Viewbags and viewdata
            ViewBag.PIDs = pids;
            ViewBag.Amounts = invoiceItems.Select(s => s.Amount).ToArray();
            ViewBag.Names = invoiceItems.Select(s => s.Product.Name).ToArray();
            ViewBag.Total = String.Format("{0:N2}", invoice.Total);
            ViewData["CompanyID"] = new SelectList(_context.Companies, "CompanyID", "CompanyName", invoice.CompanyID);
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "FullName", invoice.DebtorID);

            if (invoice == null) {
                return NotFound();
            }

            return View(invoice);
        }

        //---------------------------------

        //POST => Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            //Delete invoice from database
            await DeleteInvoice(id);
            return RedirectToAction("Index");
        }

        //---------------------------------

        //POST => Invoices/Pay/5
        public IActionResult Pay(int id) {

            //Update invoice
            Invoice invoiceBeforeUpdate = _context.Invoices.Single(s => s.InvoiceNumber == id);
            invoiceBeforeUpdate.Paid = true;

            _context.Update(invoiceBeforeUpdate);
            _context.SaveChanges();

            return RedirectToAction("Index", "MyInvoice", null);
        }

        //---------------------------------

        //GET => Invoices/Download/5
        public IActionResult Download(int id) {
            PDF pdf = new PDF(_context, _env);
            return pdf.CreatePDF(id);
        }

        //---------------------------------

        private bool InvoiceExists(int id) {
            return _context.Invoices.Any(e => e.InvoiceNumber == id);
        }

    }
}