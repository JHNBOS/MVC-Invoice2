using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WkWrap.Core;

namespace InvoiceWebApp {
    public class PDF {

        //Instances
        private ApplicationDbContext _context;
        private AppSettings _settings;
        private IHostingEnvironment _env;

        public PDF(ApplicationDbContext context, IHostingEnvironment env) {
            _context = context;
            _settings = _context.Settings.FirstOrDefault();
            _env = env;
        }

        public IActionResult CreatePDF(int id) {

            //Variables
            string invoiceNumber = "";
            string logo = Path.Combine(_env.WebRootPath, "images/" + _settings.Logo);
            
            //Instances
            Invoice invoice = _context.Invoices.Single(s => s.InvoiceNumber == id);
            Debtor debtor = _context.Debtors.Single(s => s.DebtorID == invoice.DebtorID);
            List<InvoiceItem> invoiceItems = _context.InvoiceItems.Where(s => s.InvoiceNumber == invoice.InvoiceNumber).ToList();
            List<Product> productList = new List<Product>();

            foreach (var item in invoiceItems) {
                Product product = _context.Products.Single(s => s.ProductID == item.ProductID);
                productList.Add(product);
            }

            if (_settings.Prefix != "") {
                invoiceNumber = _settings.Prefix + "-" + invoice.InvoiceNumber.ToString();
            } else {
                invoiceNumber = invoice.InvoiceNumber.ToString();
            }

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
                .invoice-box table .top td .logo {
                    padding-left: 0px !important;
                    float: left;
                    max-width: 65%;
                    height: auto;
                    text-align: left;
                }
                .invoice-box table .top .company {
                    float: right;
                    font-size: 13.5px;
                    text-align: left !important;
                    width: 30%;
                    align: right !important;
                    margin-right: 0;
                    padding-right: 10px;
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
                    text-align: center;
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
                <td class=title>";

            if (_settings.UseLogoInPDF == true) {
                companyString += "<img class=logo src=" + logo + " />";
            } else {
                companyString += "<h2>" + _settings.CompanyName + "</h2>";
            }

            companyString += 
                "</td>"
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
                + "Company ID: " + _settings.RegNumber
                + "<br /> VAT No:  " + _settings.FinancialNumber
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

            for (int i = 0; i < invoiceItems.Count; i++) {
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
                + "<td><p class=message-text> We kindly request you to pay the amount of <b>" + String.Format("&euro;{0:N2}", totalAmount) + "</b> described above before <b>" + invoice.ExpirationDate.ToString("dd-MM-yyyy") + "</b> on our bank account <b>" + _settings.BankAccountNumber + "</b> in the name of <b>" + _settings.CompanyName + "</b>, indicating the invoice number <b>" + invoiceNumber + "</b>. For questions, please contact."
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

    }
}
