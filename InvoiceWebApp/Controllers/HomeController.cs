using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InvoiceWebApp.Controllers {

    public class HomeController : Controller {

        //Instances
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context) {
            _context = context;
        }

        public IActionResult Index(string email) {

            //Current page
            ViewBag.Current = "Home";

            if (email != "") {
                User user = null;
                Admin admin = null;

                try {
                    user = _context.Users.SingleOrDefault(s => s.Email == email);
                    admin = _context.Admins.SingleOrDefault(s => s.Email == email);
                } catch (Exception ex) {
                    Debug.WriteLine(ex);
                }

                if (user != null) {
                    GetClientPage(email);
                } else if (admin != null) {
                    GetAdminPage();
                }
            }

            return View();
        }

        public IActionResult About() {

            //Current page
            ViewBag.Current = "About";

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact() {

            //Current page
            ViewBag.Current = "Contact";

            return View();
        }

        public IActionResult Error() {
            return View();
        }

        //Get invoices based on logged in client
        private void GetClientPage(string email) {
            List<Invoice> invoiceList = null;
            int paidCount = 0;
            int unpaidCount = 0;

            try {
                //Get all final invoices paid and not paid with type of final
                invoiceList = _context.Invoices
                    .Where(inv => inv.Debtor.Email == email && inv.Type == "Final").ToList();

                paidCount = invoiceList.Where(inv => inv.Paid == true).Count();
                unpaidCount = invoiceList.Where(inv => inv.Paid == false).Count();

                ViewBag.clientPaidCount = paidCount;
                ViewBag.clientNotPaidCount = unpaidCount;
                ViewBag.clientInvoices = invoiceList;

                ViewBag.clientPaid = String.Format("{0:N2}", CalculatePaid(invoiceList));
                ViewBag.clientNotPaid = String.Format("{0:N2}", CalculateToBePaid(invoiceList));
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Get invoices based on logged in client
        private void GetAdminPage() {
            List<Invoice> invoiceList = null;
            int paidCount = 0;
            int unpaidCount = 0;
            int conceptCount = 0;
            int finalCount = 0;

            try {
                //Get all invoices
                invoiceList = _context.Invoices.ToList();

                paidCount = invoiceList.Where(inv => inv.Paid == true && inv.Type == "Final").Count();
                unpaidCount = invoiceList.Where(inv => inv.Paid == false && inv.Type == "Final").Count();
                conceptCount = invoiceList.Where(inv => inv.Type == "Concept").Count();
                finalCount = invoiceList.Where(inv => inv.Type == "Final").Count();

                ViewBag.conceptCount = conceptCount;
                ViewBag.finalCount = finalCount;

                ViewBag.adminPaidCount = paidCount;
                ViewBag.adminNotPaidCount = unpaidCount;

                ViewBag.total = CalculateTotal(invoiceList);
                ViewBag.totalPaid = CalculatePaid(invoiceList);
                ViewBag.totalNotPaid = CalculateToBePaid(invoiceList);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Calculate the amount to be paid
        private decimal CalculateToBePaid(List<Invoice> invoices) {
            decimal total = 0;

            foreach (var invoice in invoices) {
                if (invoice.Type == "Final" && invoice.Paid == false) {
                    total += invoice.Total;
                }
            }
            return total;
        }

        //Calculate the amount of already paid invoices
        private decimal CalculatePaid(List<Invoice> invoices) {
            decimal total = 0;

            foreach (var invoice in invoices) {
                if (invoice.Type == "Final" && invoice.Paid == true) {
                    total += invoice.Total;
                }
            }
            return total;
        }

        //Calculate the total amount of all invoices
        private decimal CalculateTotal(List<Invoice> invoices) {
            decimal total = 0;

            foreach (var invoice in invoices) {
                if (invoice.Type == "Final") {
                    total += invoice.Total;
                }
            }
            return total;
        }


    }
}