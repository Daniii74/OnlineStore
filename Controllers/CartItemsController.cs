
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;
using OnlineStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

public class CartItemsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: CARTITEMS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.CartItems
            .Include(c => c.Product)
            .ToListAsync());
    }

    // GET: CARTITEMS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cartitem = await _context.CartItems
            .FirstOrDefaultAsync(m => m.CartItemId == id);
        if (cartitem == null)
        {
            return NotFound();
        }

        return View(cartitem);
    }

    // GET: CARTITEMS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CARTITEMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int id)
    {
        if(!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        var user = await _userManager.GetUserAsync(User);
        var existingCartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.ProductId == id && c.UserId == user.Id);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity++;
            _context.Update(existingCartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            var cartitem = new CartItem
            {
                ProductId = id,
                UserId = user.Id,
                Quantity = 1
            };
            if(ModelState.IsValid)
            {
                _context.Add(cartitem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Unable to add item to cart.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                    Console.WriteLine(cartitem.ToString());
                }
                return View(cartitem);
            }
        }
    }

    // GET: CARTITEMS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cartitem = await _context.CartItems.FindAsync(id);
        if (cartitem == null)
        {
            return NotFound();
        }
        return View(cartitem);
    }

    // POST: CARTITEMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("id,UserId,ProductId,Quantity,Product")] CartItem cartitem)
    {
        if (id != cartitem.CartItemId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(cartitem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(cartitem.CartItemId))
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
        return View(cartitem);
    }

    // GET: CARTITEMS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cartitem = await _context.CartItems
            .FirstOrDefaultAsync(m => m.CartItemId == id);
        if (cartitem == null)
        {
            return NotFound();
        }

        return View(cartitem);
    }

    // POST: CARTITEMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var cartitem = await _context.CartItems.FindAsync(id);
        if (cartitem != null)
        {
            _context.CartItems.Remove(cartitem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CartItemExists(int? id)
    {
        return _context.CartItems.Any(e => e.CartItemId == id);
    }

    public async Task<IActionResult> ClearCart()
    {
        var user = await _userManager.GetUserAsync(User);
        var cartItems = _context.CartItems.Where(c => c.UserId == user.Id);
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> IncQuantity(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem == null)
        {
            return NotFound();
        }
        cartItem.Quantity++;
        _context.Update(cartItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DecQuantity(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem == null)
        {
            return NotFound();
        }
        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            _context.Update(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}