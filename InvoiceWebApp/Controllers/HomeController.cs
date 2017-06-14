﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InvoiceWebApp.Data;
using System.Diagnostics;
using InvoiceWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceWebApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string email)
        {
            if (email != "")
            {
                GetInvoices(email);
            }

            GetPaidInvoices();
            GetConceptInvoiceCount();

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        //Get Invoices based on logged in client
        private void GetInvoices(string email)
        {
            decimal total = 0;
            List<Invoice> invoiceList = null;
            int paid = 0;
            int unpaid = 0;

            try
            {
                invoiceList = _context.Invoices.Include(inv => inv.Debtor).Where(inv => inv.Debtor.Email == email && inv.Paid == false && inv.Type != "Concept").ToList();
                paid = _context.Invoices.Include(inv => inv.Debtor).Where(inv => inv.Debtor.Email == email && inv.Paid == true && inv.Type == "Final").ToList().Count;
                unpaid = _context.Invoices.Include(inv => inv.Debtor).Where(inv => inv.Debtor.Email == email && inv.Paid == false && inv.Type == "Final").ToList().Count;

                ViewBag.paid = paid;
                ViewBag.unpaid = unpaid;
                ViewBag.invoiceList = invoiceList;

                total = CalculateTotal(invoiceList);
                ViewBag.total = String.Format("{0:N2}", total);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //Calculate the total amount of all the invoices
        private decimal CalculateTotal(List<Invoice> invoices)
        {
            decimal total = 0;

            foreach (var invoice in invoices)
            {
                total += invoice.Total;
            }

            return total;
        }

        //Get number of concept invoices
        private void GetConceptInvoiceCount()
        {
            int count = 0;

            try
            {
                List<Invoice> invoices = _context.Invoices.Where(s => s.Type == "Concept").ToList();
                count = invoices.Count;
                ViewBag.conceptCount = count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //Get all paid invoices
        private void GetPaidInvoices()
        {
            int total = 0;
            int paid = 0;
            int unpaid = 0;
            decimal totalAmount = 0;
            decimal totalPaid = 0;

            try
            {
                List<Invoice> invoices = _context.Invoices.ToList();

                total = invoices.Count();
                paid = invoices.Where(s => s.Paid == true && s.Type == "Final").Count();
                unpaid = invoices.Where(s => s.Paid == false && s.Type == "Final").Count();

                foreach (var inv in invoices.Where(s => s.Paid == true).ToList())
                {
                    totalPaid += inv.Total;
                }

                foreach (var inv in invoices.Where(s => s.Type == "Final").ToList())
                {
                    totalAmount += inv.Total;
                }

                int percent = (100 * paid) / total;

                ViewBag.totalPercentage = percent;
                ViewBag.totalPaidCount = paid;
                ViewBag.totalUnPaidCount = unpaid;

                ViewBag.totalPaid = totalPaid;
                ViewBag.totalAmount = totalAmount;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


    }
}
