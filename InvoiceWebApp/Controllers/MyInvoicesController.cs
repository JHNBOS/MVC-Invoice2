using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WkWrap.Core;

namespace InvoiceWebApp.Controllers {

    public class MyInvoicesController : Controller {

        //Instances
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;
        private readonly IViewRenderService _viewRenderService;

        public MyInvoicesController(ApplicationDbContext context, IHostingEnvironment env, IViewRenderService viewRenderService) {
            _context = context;
            _settings = _context.Settings.FirstOrDefault();
            _env = env;
            _viewRenderService = viewRenderService;
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all invoices
        private async Task<List<Invoice>> GetInvoices(string email) {
            return await _context.Invoices
                        .Include(s => s.Debtor)
                        .Include(s => s.Company)
                        .Include(s => s.InvoiceItems)
                            .ThenInclude(s => s.Product)
                        .Where(s => s.Debtor.Email == email 
                                && s.Type == "Final")
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

        //------------------------------------------------------------------------
        //Controller actions

        //GET => MyInvoices/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "MyInvoices";

            //Set session variable
            var currentUser = SessionHelper.Get<User>(this.HttpContext.Session, "User");

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.TotalSortParm = sortOrder == "Total" ? "total_desc" : "Total";
            ViewBag.DebtorSortParm = sortOrder == "Debtor" ? "debtor_desc" : "Debtor";

            //Search function
            var invoices = await GetInvoices(currentUser.Email);
            var query = from invoice in invoices
                        select invoice;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(i => i.InvoiceNumber.ToString().Contains(searchQuery)
                                    || i.Total.ToString().Contains(searchQuery));
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

        //GET => MyInvoices/Details/5
        public async Task<IActionResult> Details(int? id) {
            
            //Current page
            ViewBag.Current = "MyInvoices";

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
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "FullName", invoice.DebtorID);

            if (invoice == null) {
                return NotFound();
            }

            return View(invoice);
        }

        //---------------------------------

        //POST => MyInvoices/Pay/5
        public IActionResult Pay(int id) {

            //Update invoice
            Invoice invoiceBeforeUpdate = _context.Invoices.Single(s => s.InvoiceNumber == id);
            invoiceBeforeUpdate.Paid = true;

            _context.Update(invoiceBeforeUpdate);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //---------------------------------

        //GET => MyInvoices/Download/5
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