
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineStore.Controllers;
using Microsoft.AspNetCore.Authorization;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PRODUCTS
    public async Task<IActionResult> Index()  
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
        return View(products);
    }

    // GET: PRODUCTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
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

    // GET: PRODUCTS/Create
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
        return View();
    }

    // POST: PRODUCTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile Image)
    {
        UploadHandler uploadHandler = new UploadHandler();
        string imagePath = uploadHandler.imageHandler(Image);

        product.ImageUrl = imagePath;

        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                }
            }
        }
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
        return View(product);
    }

    // GET: PRODUCTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryId", "Name");
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // POST: PRODUCTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Product product, IFormFile image)
    {
        UploadHandler uploadHandler = new UploadHandler();
        Product existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
        var oldImagePath = existingProduct.ImageUrl;
        if (image != null)
        {
            string imagePath = uploadHandler.imageHandler(image);
            product.ImageUrl = imagePath;
            
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                var oldImageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImagePath.TrimStart('/'));
                Console.WriteLine($"Deleting old image: {oldImageFullPath}");
                System.IO.File.Delete(oldImageFullPath);
                
            }
        }
        if (id != product.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
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
        return RedirectToAction(nameof(Index));
    }

    // GET: PRODUCTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(m => m.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: PRODUCTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _context.Products.Remove(product);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int? id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }
}
