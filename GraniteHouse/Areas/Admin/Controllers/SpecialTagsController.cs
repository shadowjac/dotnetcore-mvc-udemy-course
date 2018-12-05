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
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public SpecialTagsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(await dbContext.SpecialTags.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTags tag)
        {
            if (ModelState.IsValid)
            {
                dbContext.SpecialTags.Add(tag);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();
            var tag = await dbContext.SpecialTags.FindAsync(id.Value);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTags tag)
        {
            if (tag == null) return BadRequest();
            if (id != tag.Id) return NotFound();

            if (ModelState.IsValid)
            {
                dbContext.Update(tag);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();
            var tag = await dbContext.SpecialTags.FindAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();
            var tag = await dbContext.SpecialTags.FindAsync(id.Value);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await dbContext.SpecialTags.FindAsync(id);
            dbContext.SpecialTags.Remove(tag);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}