using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using System.Diagnostics;

namespace InvoiceWebApp.Controllers {

    public class CategoriesController : Controller {

        //Instances
        private ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context){
            _context = context;    
        }

        //------------------------------------------------------------------------
        //Database action methods

        //Get a list of all categories
        private async Task<List<Category>> GetCategories() {
            return await _context.Categories.ToListAsync();
        }

        //Get category based on id
        private async Task<Category> GetCategory(int? id) {
            Category category = null;

            try {
                category = await _context.Categories
                    .Include(s => s.Products)
                    .SingleOrDefaultAsync(s => s.CategoryID == id);
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }

            return category;
        }

        //Add category to the database
        private async Task CreateCategory(Category category) {

            try {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //Update existing category
        private async Task UpdateCategory(Category category) {
            try {
                _context.Update(category);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                Debug.WriteLine(ex);
            }
        }

        //Remove existing category from the database
        private async Task DeleteCategory(int id) {
            Category category = await GetCategory(id);

            try {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
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
            var categories = await GetCategories();
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
            //Current page
            ViewBag.Current = "Categories";

            if (id == null)
            {
                return NotFound();
            }

            var category = await GetCategory(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            //Current page
            ViewBag.Current = "Categories";

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
                await CreateCategory(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //Current page
            ViewBag.Current = "Categories";

            if (id == null)
            {
                return NotFound();
            }

            var category = await GetCategory(id);

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
                    await UpdateCategory(category);
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
            //Current page
            ViewBag.Current = "Categories";

            if (id == null)
            {
                return NotFound();
            }

            var category = await GetCategory(id);

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
            await DeleteCategory(id);
            return RedirectToAction("Index");
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
    }
}
