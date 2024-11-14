using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBite.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var restaurantId = Guid.Parse(HttpContext.Session.GetString("RestaurantId"));
            
            var products = _context.Products.Include(p => p.Category)
                .Where(r => r.RestaurantId == restaurantId);
            return View(await products.ToListAsync());
        }

        [AllowAnonymous]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Add a Menu Item";
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Price,CategoryId,Description")] Product product, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                // check for and upload & rename photo if there is one
                if (Photo != null)
                {
                    var fileName = UploadPhoto(Photo);
                    product.Photo = fileName;
                }
                product.Restaurant = await _context.Restaurant.FindAsync(Guid.Parse(HttpContext.Session.GetString("RestaurantId")));
                product.ProductId = Guid.NewGuid(); // Assign a new GUID
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductId,Name,Price,CategoryId,Description")] Product product, IFormFile? Photo, string? CurrentPhoto)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Photo != null)
                    {
                        var fileName = UploadPhoto(Photo);
                        product.Photo = fileName;
                    }
                    else if (CurrentPhoto != null)
                    {
                        product.Photo = CurrentPhoto;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products' is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products?.Any(e => e.ProductId == id) ?? false;
        }

        private string UploadPhoto(IFormFile Photo)
        {
            var filePath = Path.GetTempFileName();
            var fileName = Guid.NewGuid() + "-" + Photo.FileName;
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product-uploads", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }

            return fileName;
        }
    }
}
