
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.Data;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: CATEGORYS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Categories.ToListAsync());
    }

    // GET: CATEGORYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.CategoryId == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: CATEGORYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CATEGORYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, IFormFile image)
    {
        
        UploadHandler uploadHandler = new UploadHandler();

        string imagePath = uploadHandler.imageHandler(image);

        category.ImageUrl = imagePath;

        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
        }
        return View(category);
    }

    // GET: CATEGORYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    // POST: CATEGORYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Category category, IFormFile image)
    {
        var existingCategory = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);
        var existingImageUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCategory.ImageUrl.TrimStart('/'));

        if (image != null && image.Length > 0)
        {
            UploadHandler uploadHandler = new UploadHandler();
            string imagePath = uploadHandler.imageHandler(image);
            category.ImageUrl = imagePath;
            System.IO.File.Delete(existingImageUrl);
        }

        if (id != category.CategoryId)
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
                if (!CategoryExists(category.CategoryId))
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
        return View(category);
    }

    // GET: CATEGORYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.CategoryId == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: CATEGORYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int? id)
    {
        return _context.Categories.Any(e => e.CategoryId == id);
    }
}
