using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GraniteHouse.Models;
using GraniteHouse.Data;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.Extensions;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var products = await dbContext.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await dbContext.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags).SingleOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Details")]
        public async Task<IActionResult> DetailsPost(int id)
        {
            var lstCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstCart == null) lstCart = new List<int>();
            lstCart.Add(id);
            HttpContext.Session.Set("ssShoppingCart", lstCart);

            return RedirectToAction(nameof(Index), "Home", new { area = "Customer" });
        }

        public async Task<IActionResult> Remove(int id)
        {
            var lstCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstCart.Any())
            {
                if (lstCart.Contains(id))
                    lstCart.Remove(id);
            }
            HttpContext.Session.Set<List<int>>("ssShoppingCart", lstCart);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
