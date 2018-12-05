using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ProductTypesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await dbContext.ProductTypes.ToListAsync());
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes productTypes)
        {
            if (ModelState.IsValid)
            {
                dbContext.ProductTypes.Add(productTypes);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        //GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();
            var pt = await dbContext.ProductTypes.FindAsync(id.Value);
            if (pt == null) return NotFound();
            return View(pt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypes productTypes)
        {
            if (id != productTypes.Id) return NotFound();

            if (ModelState.IsValid)
            {
                dbContext.Update(productTypes);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productTypes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();
            var pt = await dbContext.ProductTypes.FindAsync(id.Value);
            if (pt == null) return NotFound();
            return View(pt);
        }

        //GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();
            var pt = await dbContext.ProductTypes.FindAsync(id);
            if (pt == null) return NotFound();
            return View(pt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pt = await dbContext.ProductTypes.FindAsync(id);
            dbContext.Remove(pt);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}