using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InvoiceWebApp.Data;
using InvoiceWebApp.Models;

namespace InvoiceWebApp.Controllers {

    public class CategoriesController : Controller {

        //Instances
        private ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context){
            _context = context;    
        }

        //------------------------------------------------------------------------
        //Controller actions

        // GET: Categories
        public async Task<IActionResult> Index(string sortOrder, string searchQuery)
        {
            //Current page
            ViewBag.Current = "Categories";

            //Sort function
            ViewBag.BeginSortParm = String.IsNullOrEmpty(sortOrder) ? "begin_desc" : "";
            ViewBag.CategoryNameSortParm = sortOrder == "CategoryName" ? "categoryname_desc" : "CategoryName";

            //Search function
            var categories = await _context.Categories.ToListAsync();
            var query = from category in categories
                        select category;

            if (!String.IsNullOrEmpty(searchQuery)) {
                query = query.Where(s => s.CategoryName.Contains(searchQuery));
            }

            switch (sortOrder) {
                //Default
                case "begin_desc":
                    query = query.OrderByDescending(s => s.CategoryName);
                    break;

                //Sort on name
                case "CategoryName":
                    query = query.OrderBy(s => s.CategoryName);
                    break;
                case "categoryname_desc":
                    query = query.OrderByDescending(s => s.CategoryName);
                    break;

                //Default
                default:
                    query = query.OrderBy(s => s.CategoryName);
                    break;
            }

            return View(query);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(s => s.Products)
                .SingleOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(s => s.Products)
                .SingleOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryID,CategoryName")] Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryID))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(s => s.Products)
                .SingleOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                                .Include(s => s.Products)
                                .SingleOrDefaultAsync(m => m.CategoryID == id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
    }
}
