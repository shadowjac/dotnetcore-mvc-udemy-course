using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly HostingEnvironment hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProductsViewModel { get; set; }

        public ProductsController(ApplicationDbContext dbContext, HostingEnvironment hostingEnvironment)
        {
            this.dbContext = dbContext;
            this.hostingEnvironment = hostingEnvironment;
            ProductsViewModel = new ProductsViewModel
            {
                ProductTypes = dbContext.ProductTypes.ToList(),
                SpecialTags = dbContext.SpecialTags.ToList(),
                Products = new Models.Products()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = dbContext.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags);
            return View(await products.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(ProductsViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Create")]
        public async Task<IActionResult> CreateConfirm()
        {
            if (!ModelState.IsValid) return View(ProductsViewModel);
            dbContext.Products.Add(ProductsViewModel.Products);
            await dbContext.SaveChangesAsync();

            //Image
            var webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var productsFromDb = await dbContext.Products.FindAsync(ProductsViewModel.Products.Id);

            if (files.Any())
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files.First().FileName);
                using (var fileStream = new FileStream(Path.Combine(uploads, ProductsViewModel.Products.Id + extension), FileMode.Create))
                {
                    await files.First().CopyToAsync(fileStream);
                }
                productsFromDb.Image = $@"\{SD.ImageFolder}\{ProductsViewModel.Products.Id}{extension}";
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductsViewModel.Products.Id + ".jpg");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductsViewModel.Products.Id + ".jpg";
            }

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();
            ProductsViewModel.Products = await dbContext.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id.Value);
            if (ProductsViewModel.Products == null) return NotFound();
            return View(ProductsViewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = await dbContext.Products.SingleOrDefaultAsync(m => m.Id == ProductsViewModel.Products.Id);

                if (files != null && files.Any() && files[0].Length > 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsViewModel.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsViewModel.Products.Id + extension_old));
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, ProductsViewModel.Products.Id + extension_new), FileMode.Create))
                    {
                        await files.First().CopyToAsync(fileStream);
                    }
                    ProductsViewModel.Products.Image = $@"\{SD.ImageFolder}\{ProductsViewModel.Products.Id}{extension_new}";
                }

                if (!string.IsNullOrEmpty(ProductsViewModel.Products.Image))
                {
                    productFromDb.Image = ProductsViewModel.Products.Image;
                }
                productFromDb.Name = ProductsViewModel.Products.Name;
                productFromDb.Price = ProductsViewModel.Products.Price;
                productFromDb.Available = ProductsViewModel.Products.Available;
                productFromDb.ProductTypeId = ProductsViewModel.Products.ProductTypeId;
                productFromDb.ShadeColor = ProductsViewModel.Products.ShadeColor;
                productFromDb.SpecialTagId = ProductsViewModel.Products.SpecialTagId;

                await dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ProductsViewModel);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();
            ProductsViewModel.Products = await dbContext.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id.Value);
            if (ProductsViewModel.Products == null) return NotFound();
            return View(ProductsViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();
            ProductsViewModel.Products = await dbContext.Products.Include(m => m.SpecialTags).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id.Value);
            if (ProductsViewModel.Products == null) return NotFound();
            return View(ProductsViewModel);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var webRootPath = hostingEnvironment.WebRootPath;
            var product = await dbContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            var uploads = Path.Combine(webRootPath, SD.ImageFolder);
            var extension = Path.GetExtension(product.Image);

            dbContext.Products.Remove(product);
            if(System.IO.File.Exists(Path.Combine(uploads, product.Id + extension)))
            {
                System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));
            }
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}