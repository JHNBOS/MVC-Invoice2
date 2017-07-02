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

    public class ProductsController : Controller {

        //Instances
        private ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context) {
            _context = context;
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all products
        private async Task<List<Product>> GetProducts() {
            return await _context.Products.Include(s => s.InvoiceItems).ToListAsync();
        }

        //Get product based on id
        private async Task<Product> GetProduct(int? id) {
            Product product = null;

            try {
                product = await _context.Products.SingleOrDefaultAsync(s => s.ProductID == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return product;
        }

        //Add product to the database
        private async Task CreateProduct(Product product, string price) {
            //Parse price from string to decimal
            product.Price = decimal.Parse(price);

            try {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing debtor
        private async Task UpdateProduct(Product product, string price) {
            //Parse price from string to decimal
            product.Price = decimal.Parse(price);

            try {
                _context.Update(product);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing product and all related invoice items from the database
        private async Task DeleteProduct(int id) {
            Product product = await GetProduct(id);

            try {
                _context.InvoiceItems
                    .RemoveRange(_context.InvoiceItems.Where(s => s.ProductID == product.ProductID).ToList());
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //------------------------------------------------------------------------
        //Controller actions

        //GET => Products/Index
        public async Task<IActionResult> Index(string sortOrder, string searchQuery) {

            //Current page
            ViewBag.Current = "Products";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            //Search function
            var products = await GetProducts();
            var query = from product in products
                        select product;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.Name.Contains(searchQuery)
                                    || s.Price.ToString().Contains(searchQuery));
            }

            switch (sortOrder) {
                //Default
                case "begin_desc":
                    query = query.OrderByDescending(s => s.Name);
                    break;

                //Sort on name
                case "Name":
                    query = query.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(s => s.Name);
                    break;

                //Sort on price
                case "Price":
                    query = query.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(s => s.Price);
                    break;

                //Default
                default:
                    query = query.OrderBy(s => s.Name);
                    break;
            }

            return View(query);
        }

        //---------------------------------

        //GET => Products/Details/5
        public async Task<IActionResult> Details(int? id) {

            //Current page
            ViewBag.Current = "Products";

            if (id == null) {
                return NotFound();
            }

            //Get product
            var product = await GetProduct(id);

            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        //---------------------------------

        //GET => Products/Create
        public IActionResult Create() {

            //Current page
            ViewBag.Current = "Products";

            return View();
        }

        //---------------------------------

        //POST => Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Description,Name,VAT")] Product product, string price) {

            if (ModelState.IsValid) {
                //Add product to database
                await CreateProduct(product, price);
                return RedirectToAction("Index", "Products", null);
            }

            return View(product);
        }

        //---------------------------------

        //GET => Products/Edit/5        
        public async Task<IActionResult> Edit(int? id) {

            //Current page
            ViewBag.Current = "Products";

            if (id == null) {
                return NotFound();
            }

            //Get product
            var product = await GetProduct(id);

            //Set viewbag variables
            ViewBag.Price = String.Format("{0:N2}", product.Price);
            ViewBag.VAT = product.VAT;

            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        //---------------------------------

        //POST => Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Description,Name,VAT")] Product product, string price) {

            if (id != product.ProductID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    //Update product
                    await UpdateProduct(product, price);
                } catch (DbUpdateConcurrencyException) {
                    if (!ProductExists(product.ProductID)) {
                        return NotFound();
                    } else { throw; }
                }

                return RedirectToAction("Index", "Products", null);
            }

            return View(product);
        }

        //---------------------------------

        //GET => Products/Delete/5
        public async Task<IActionResult> Delete(int? id) {

            //Current page
            ViewBag.Current = "Products";

            if (id == null) {
                return NotFound();
            }

            //Get product
            var product = await GetProduct(id);

            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        //---------------------------------

        //POST => Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {

            //Delete product from database
            await DeleteProduct(id);
            return RedirectToAction("Index", "Products", null);
        }

        //---------------------------------

        private bool ProductExists(int id) {
            return _context.Products.Any(e => e.ProductID == id);
        }

    }
}