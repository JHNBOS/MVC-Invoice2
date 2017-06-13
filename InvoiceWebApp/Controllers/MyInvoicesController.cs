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
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using WkWrap.Core;
using System.Collections;

namespace InvoiceWebApp.Controllers
{
    public class MyInvoicesController : Controller
    {
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;
        private readonly IViewRenderService _viewRenderService;

        public MyInvoicesController(ApplicationDbContext context, IHostingEnvironment env, IViewRenderService viewRenderService)
        {
            _context = context;
            _settings = _context.Settings.FirstOrDefault();
            _env = env;
            _viewRenderService = viewRenderService;
        }

        /*----------------------------------------------------------------------*/
        //DATABASE ACTION METHODS

        private async Task<Invoice> GetInvoice(int? id)
        {
            Invoice invoice = null;

            try
            {
                invoice = await _context.Invoices.Include(s => s.Debtor)
                                    .Include(s => s.InvoiceItems)
                                    .ThenInclude(s => s.Product)
                                    .SingleOrDefaultAsync(s => s.InvoiceNumber == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return invoice;
        }

        private async Task<List<InvoiceItem>> GetInvoiceItems(int? id)
        {
            List<InvoiceItem> itemList = null;

            try
            {
                itemList = await _context.InvoiceItems.Include(d => d.Product)
                                        .Where(s => s.InvoiceNumber == id)
                                        .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return itemList;
        }

        /*----------------------------------------------------------------------*/
        //CONTROLLER ACTIONS

        // GET: MyInvoice
        public async Task<IActionResult> Index(string sortOrder, string searchQuery)
        {
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

            if (!String.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(i => i.InvoiceNumber.ToString().Contains(searchQuery)
                                    || i.Total.ToString().Contains(searchQuery));
            }

            switch (sortOrder)
            {
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Invoice invoice = await GetInvoice(id);
            List<InvoiceItem> invoiceItems = await GetInvoiceItems(id);
            List<Product> productList = new List<Product>();

            if (invoice == null)
            {
                return NotFound();
            }
            foreach (var item in invoiceItems)
            {
                Product product = _context.Products.SingleOrDefault(s => s.ProductID == item.ProductID);
                productList.Add(product);
            }

            int cnt = 0;
            string[] pids = new string[invoiceItems.Count];

            foreach (var product in productList)
            {
                string _id = product.ProductID + "_" + product.Price;
                pids[cnt] = _id;
                cnt++;
            }

            ViewBag.PIDs = pids;
            ViewBag.Amounts = invoiceItems.Select(s => s.Amount).ToArray();
            ViewBag.Names = invoiceItems.Select(s => s.Product.Name).ToArray();
            ViewBag.Total = String.Format("{0:N2}", invoice.Total);
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "FullName", invoice.DebtorID);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: MyInvoice/Create
        public IActionResult Create()
        {
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address");
            return View();
        }

        // POST: MyInvoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceNumber,CreatedOn,DebtorID,ExpirationDate,Total,Type")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address", invoice.DebtorID);
            return View(invoice);
        }

        // GET: MyInvoice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceNumber == id);
            if (invoice == null)
            {
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
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceNumber,CreatedOn,DebtorID,ExpirationDate,Total,Type")] Invoice invoice)
        {
            if (id != invoice.InvoiceNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["DebtorID"] = new SelectList(_context.Debtors, "DebtorID", "Address", invoice.DebtorID);
            return View(invoice);
        }

        // GET: MyInvoice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceNumber == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: MyInvoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.SingleOrDefaultAsync(m => m.InvoiceNumber == id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult DownloadPDF(int id)
        {
            Invoice invoice = _context.Invoices.Single(s => s.InvoiceNumber == id);
            Debtor debtor = _context.Debtors.Single(s => s.DebtorID == invoice.DebtorID);
            List<InvoiceItem> invoiceItems = _context.InvoiceItems.Where(s => s.InvoiceNumber == invoice.InvoiceNumber).ToList();
            List<Product> productList = new List<Product>();

            foreach (var item in invoiceItems)
            {
                Product product = _context.Products.Single(s => s.ProductID == item.ProductID);
                productList.Add(product);
            }

            string invoiceNumber = _settings.Prefix + "-" + invoice.InvoiceNumber.ToString();

            //CSS string
            var cssString = @"<style> 
                .invoice-box { 
                    max-width: 2480px;
                    max-height: 3495px:
                    width: 100%;
                    height: 90%;
                    margin: auto 0;
                    padding: 30px 7px;
                    font-size: 15px;
                    line-height: 24px;
                    font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;
                    color: #555;
                    letter-spacing: 0px;
                } 
                .invoice-box table { 
                    width: 100%;
                    line-height: inherit;
                    text-align: left;
                } 
                .invoice-box table td { 
                    padding: 5px 0px;
                    vertical-align: top;
                } 
                .invoice-box table tr.top table td { 
                    padding-bottom: 50px; 
                }
                .invoice-box table .top td .title { 
                    font-size: 32px;
                    line-height: 32px;
                    padding-left: 65px !important;
                    color: #333;
                    float: left;
                    width: 80%;
                    text-align: left;
                } 
                .invoice-box table .top .company { 
                    float: right;
                    font-size: 13.5px;
                    text-align: left !important;
                    width: 20%;
                    align: right !important;
                    margin-right: 0;
                    padding-right: 5px;
                    margin-left: 10px;
                }
                .invoice-box table .top .company hr{
                    margin-left: auto;
                    margin-right: auto;
                    width: 100%;
                    height: 2px;
                    margin-top: 2px;
                    margin-bottom: 0;
                    padding-bottom: 1px;
                }
                .invoice-box .debtor-table{
                    margin-left: 55px !important;
                    margin-bottom: 45px !important;
                }
                .invoice-box .debtor-table .debtor-info{
                    border-collapse: collapse;
                    border-spacing; 0;
                    padding: 0 0 0 0;
                    margin: 0 0 0 15px;
                }
                .invoice-box .debtor-table .debtor-info tr{
                    padding: 0 0 0 0;
                    margin: 0 0 0 0;
                }
                .invoice-box .debtor-table .debtor-name,
                .invoice-box .debtor-table .debtor-address,
                .invoice-box .debtor-table .debtor-city{ 
                    font-size: 16px;
                    text-align: left !important;
                    padding-bottom: 0px;
                    line-height: normal;
                }
                .invoice-box .invoice-info{
                    border-collapse: collapse;
                    border-spacing; 0;
                    padding: 0 0 0 0;
                    margin: 8px 0 65px 55px !important;
                    width: 30%;
                }
                .invoice-box .invoice-info tr{
                    margin: 0 0 0 0 !important;
                    padding: 0 0 0 0;
                    line-height: 64%;
                    font-size: 16px;
                    border-collapse: collapse;
                    border-spacing; 0;
                }
                .invoice-box .invoice-info tr td:nth-child(1){
                    width: 40%;
                }
                .invoice-box .invoice-info tr td:nth-child(2){
                    width: 60%
                    padding-left: 2px;
                    text-align: left;
                    align: left;
                }
                .invoice-box .item-table tr.heading td { 
                    background: #eee; 
                    border-bottom: 1px solid #ddd; 
                    font-weight: bold;
                    font-size: 15px;
                } 
                .invoice-box .item-table tr.details td { 
                    padding-bottom: 20px; 
                } 
                .invoice-box .item-table{
                    margin-bottom: 25px !important;
                }
                .invoice-box .item-table tr.item td { 
                    border-bottom: 1px solid #eee;
                    font-size: 14px;
                } 
                .invoice-box .item-table tr.item.last td { 
                    border-bottom: none; 
                }
                .invoice-box .item-table tr td:nth-child(1){ 
                    border-top: 2px solid #eee; 
                    width: 26%;
                    padding-left: 4px;
                } 
                .invoice-box .item-table tr td:nth-child(2){ 
                    width: 37%;
                } 
                .invoice-box .item-table tr td:nth-child(4),
                .invoice-box .item-table tr td:nth-child(5),
                .invoice-box .item-table tr td:nth-child(6){
                    text-align: center;
                }
                .invoice-box .item-table tr td:nth-child(3),
                .invoice-box .item-table tr td:nth-child(4),
                .invoice-box .item-table tr td:nth-child(5){ 
                    width: 8%;
                }
                .invoice-box .item-table tr td:nth-child(6){ 
                    width: 13%
                } 
                .invoice-box .total-table{
                    border-collapse: collapse;
                    border-spacing; 0;
                    width: 18%;
                    float: right;
                    margin: 0 35px 0 0 !important;
                    padding: 0 0 0 0 !important;
                }
                .invoice-box .total-table .total{
                    border-top: 1px solid #888; 
                }
                .invoice-box .total-table tr td:nth-child(1),
                .invoice-box .total-table tr td:nth-child(2),
                .invoice-box .total-table tr td:nth-child(3){ 
                    align: right !important;
                    font-weight: bold; 
                    text-align: right;
                    font-size: 15px;
                    padding-bottom: 1px;
                } 
                .invoice-box .disclaimer-table{
                    width: 100%;
                    border-top: 2px solid #DDD;
                    margin: 0 0 0 0 !important;
                    padding: 0 0 0 0 !important;
                    position: fixed !important;
                    bottom: 4%;
                    left: 0%;
                }
                .invoice-box .disclaimer-table .message .message-text{
                    font-size: 16px;
                    text-align: left;
                    padding: 5px 25px !important;
                }
                @media only screen and (max-width: 600px) { 
                    .invoice-box table tr.top table td { 
                        width: 100%;
                        display: block;
                        text-align: center;
                    } 
                    .invoice-box table tr.information table td { 
                        width: 100%; 
                        display: block; 
                        text-align: center; 
                    } 
                } 
            </style>";

            //Company string
            string companyString = @"<div class=invoice-box>
                <table cellpadding=0 cellspacing=0> 
                <tr class=top> 
                <td colspan=2> 
                <table> 
                <tr> 
                <td class=title> 
                <h2>" + _settings.CompanyName + "</h2>"
                + "</td>"
                + "<td class=company>"
                + _settings.CompanyName
                + "<hr />"
                + _settings.Address
                + "<br />" + _settings.PostalCode + " " + _settings.City
                + "<br />" + _settings.Country
                + "<hr />"
                + _settings.Website
                + "<br />" + _settings.Phone
                + "<hr />"
                + "Financial No:  " + _settings.RegNumber
                + "<br /> VAT:  " + _settings.FinancialNumber
                + "<br /> Bank:  " + _settings.BankAccountNumber
                + "</td>"
                + "</tr> "
                + "</table> "
                + "</td>"
                + "</tr>"
                + "</table>";

            //Debtor string
            string debtorString = @"<table class=debtor-table cellpadding=0 cellspacing=0>"
                + "<tr>"
                    + "<td class=debtor-name>"
                        + debtor.FirstName + " " + debtor.LastName
                    + "</td>"
                + "</tr>"
                + "<tr>"
                    + "<td class=debtor-address>"
                        + debtor.Address
                    + "</td>"
                + "</tr>"
                + "<tr>"
                    + "<td class=debtor-city>"
                        + debtor.PostalCode + " " + debtor.City
                    + "</td>"
                + "</tr>"
            + "</table>";

            //Spacer string
            string spacerString = @"<br />";

            //Invoice info
            string invoiceString = @"<table class=invoice-info cellpadding=0 cellspacing=0>"
                + "<tr>"
                + "<td>Date: </td>"
                + "<td>" + invoice.CreatedOn.ToString("dd-MM-yyyy") + "</td>"
                + "</tr>"
                + "<tr>"
                + "<td>Invoice No: </td>"
                + "<td>" + invoiceNumber + "</td>"
                + "</tr>"
                + "</table>";

            //Product string
            string productString = @"<table class=item-table cellpadding=0 cellspacing=0>"
                + "<tr class=heading> "
                + "<td>Product</td>"
                + "<td>Description</td>"
                + "<td>Price</td>"
                + "<td>Qnt</td>"
                + "<td>VAT</td>"
                + "<td>Total</td>"
                + "</tr>";

            //Table string
            string tableString = "";
            decimal totalAmount = 0;
            decimal subTotalAmount = 0;
            decimal vatTotalAmount = 0;

            for (int i = 0; i < invoiceItems.Count; i++)
            {
                InvoiceItem item = invoiceItems[i];
                Product product = productList.Single(s => s.ProductID == item.ProductID);

                int vatPercentage = product.VAT;
                decimal subtotal = (product.Price * 100) / (100 + vatPercentage);
                decimal total = product.Price * item.Amount;

                subTotalAmount += subtotal;
                totalAmount += total;

                tableString += @"<tr class=item>"
                        + "<td>" + product.Name + "</td>"
                        + "<td>" + product.Description + "</td>"
                        + "<td>&euro; " + String.Format("{0:N2}", product.Price) + "</td>"
                        + "<td>" + item.Amount + "</td>"
                        + "<td>" + String.Format("{0}%", product.VAT) + "</td>"
                        + "<td>&euro; " + total + "</td>"
                        + "</tr>";
            }

            vatTotalAmount = (totalAmount - subTotalAmount);
            tableString += "</table>";

            //Total string
            string totalString = @"<table class=total-table cellspacing=0 cellpadding=0>"
                + "<tr class=vat>"
                + "<td>VAT:  &euro; " + String.Format("{0:N2}", vatTotalAmount) + "</td>"
                + "</tr>"
                + "<tr class=subtotal>"
                + "<td>Subtotal:  &euro; " + String.Format("{0:N2}", subTotalAmount) + "</td>"
                + "</tr>"
                + "<tr class=total>"
                + "<td>Total:  &euro; " + String.Format("{0:N2}", totalAmount) + "</td>"
                + "</tr>"
                + "</table>";

            //Disclaimer string
            string disclaimerString = @"<table class=disclaimer-table cellspacing=0 cellpadding=0>"
                + "<tr class=message>"
                + "<td><p class=message-text> We kindly request you to pay the amount of <b>" + String.Format("&euro;{0:N2}", totalAmount) + "</b> described above before <b>" + invoice.ExpirationDate.ToString("dd-MM-yyyy") + "</b> on our bank account indicating the invoice number <b>" + invoiceNumber + "</b>. For questions, please contact."
                + "</p></td>"
                + "</tr>"
                + "</table>"
                + "</div>";

            //Full HTML string
            string htmlContent = cssString + companyString + debtorString + spacerString
                + invoiceString + productString + tableString + totalString + disclaimerString;

            var wkhtmltopdf = new FileInfo(@"wkhtmltopdf\bin\wkhtmltopdf.exe");
            var converter = new HtmlToPdfConverter(wkhtmltopdf);
            var pdfBytes = converter.ConvertToPdf(htmlContent);

            FileResult fileResult = new FileContentResult(pdfBytes, "application/pdf");
            fileResult.FileDownloadName = "invoice-" + id + ".pdf";
            return fileResult;
        }

        // POST: Invoice/Pay
        public IActionResult Pay(int id)
        {
            Invoice invoiceBeforeUpdate = _context.Invoices.Single(s => s.InvoiceNumber == id);
            invoiceBeforeUpdate.Paid = true;

            _context.Update(invoiceBeforeUpdate);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceNumber == id);
        }

    }
}
 