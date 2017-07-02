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

        /*----------------------------------------------------------------------*/
        //DATABASE ACTION METHODS

        private async Task<Invoice> GetInvoice(int? id) {
            Invoice invoice = null;

            try {
                invoice = await _context.Invoices.Include(s => s.Debtor)
                                    .Include(s => s.InvoiceItems)
                                    .ThenInclude(s => s.Product)
                                    .SingleOrDefaultAsync(s => s.InvoiceNumber == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return invoice;
        }

        private async Task<List<InvoiceItem>> GetInvoiceItems(int? id) {
            List<InvoiceItem> itemList = null;

            try {
                itemList = await _context.InvoiceItems.Include(d => d.Product)
                                        .Where(s => s.InvoiceNumber == id)
                                        .ToListAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return itemList;
        }

        /*----------------------------------------------------------------------*/
        //CONTROLLER ACTIONS

        // GET: MyInvoice
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {
            //CURRENT PAGE
            ViewBag.Current = "MyInvoices";

            var currentUser = SessionHelper.Get<User>(this.HttpContext.Session, "User");
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";

            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
            ViewBag.TotalSortParm = sortOrder == "Total" ? "total_desc" : "Total";
            ViewBag.DebtorSortParm = sortOrder == "Debtor" ? "debtor_desc" : "Debtor";

            var invoices = _context.Invoices
                .Include(i => i.Debtor)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(c => c.Product)
                .Where(d => d.Debtor.Email == currentUser.Email && d.Type == "Final");

            var query = from invoice in invoices
                        select invoice;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(i => i.InvoiceNumber.ToString().Contains(searchQuery)
                                    || i.Total.ToString().Contains(searchQuery));
            }

            switch (sortOrder) {
                //WHEN NO SORT
                case "begin_desc":
                    query = query.OrderBy(s => s.InvoiceNumber);
                    break;
                //INVOICE NUMBER
                case "Number":
                    query = query.OrderBy(s => s.InvoiceNumber);
                    break;

                case "number_desc":
                    query = query.OrderByDescending(s => s.InvoiceNumber);
                    break;
                //TOTAL
                case "Total":
                    query = query.OrderBy(s => s.Total);
                    break;

                case "total_desc":
                    query = query.OrderByDescending(s => s.Total);
                    break;
                //DEFAUlT
                default:
                    query = query.OrderBy(s => s.InvoiceNumber);
                    break;
            }

            return View(await query.ToListAsync());
        }

        // GET: Invoice/Details/5
        public async Task<IActionResult> Details(int? id) {
            //CURRENT PAGE
            ViewBag.Current = "MyInvoices";

            if (id == null) {
                return NotFound();
            }

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

        // GET: MyInvoice/Create
        public IActionResult Create() {
            //CURRENT PAGE
            ViewBag.Current = "MyInvoices";

            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address");
            return View();
        }

        // POST: MyInvoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceNumber,CreatedOn,DebtorID,ExpirationDate,Total,Type")] Invoice invoice) {
            if (ModelState.IsValid) {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address", invoice.DebtorID);
            return View(invoice);
        }

        // GET: MyInvoice/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            //CURRENT PAGE
            ViewBag.Current = "MyInvoices";

            if (id == null) {
                return NotFound();
            }

            var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceNumber == id);
            if (invoice == null) {
                return NotFound();
            }
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address", invoice.DebtorID);
            return View(invoice);
        }

        // POST: MyInvoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceNumber,CreatedOn,DebtorID,ExpirationDate,Total,Type")] Invoice invoice) {
            if (id != invoice.InvoiceNumber) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!InvoiceExists(invoice.InvoiceNumber)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address", invoice.DebtorID);
            return View(invoice);
        }

        // GET: MyInvoice/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            //CURRENT PAGE
            ViewBag.Current = "MyInvoices";

            if (id == null) {
                return NotFound();
            }

            var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceNumber == id);
            if (invoice == null) {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: MyInvoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceNumber == id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult DownloadPDF(int id) {
            PDF pdf = new PDF(_context);
            return pdf.CreatePDF(id);
        }

        // POST: Invoice/Pay
        public IActionResult Pay(int id) {
            Invoice invoiceBeforeUpdate = _context.Invoices.Single(s => s.InvoiceNumber == id);
            invoiceBeforeUpdate.Paid = true;

            _context.Update(invoiceBeforeUpdate);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool InvoiceExists(int id) {
            return _context.Invoices.Any(e => e.InvoiceNumber == id);
        }
    }
}