using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Where(o => o.UserId == _userManager.GetUserId(User))
                .ToList();
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _context.OrderDetails
                .Where(od => od.OrderId == id)
                .Include(od => od.Product)
                .ToList();
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [Authorize]
        [HttpPost]
        public IActionResult PlaceOrder()
        {
            var user = _userManager.GetUserId(User);
            var cartItems = _context.CartItems
                .Where(c => c.UserId == user)
                .Include(c => c.Product)
                .ToList();

            var order = new Order
            {
                UserId = user,
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentStatus = "Pending",
                OrderDetails = new List<OrderDetail>()
            };

            decimal totalAmount = 0;

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price * item.Quantity
                };
                order.OrderDetails.Add(orderDetail);
                totalAmount += item.Quantity * item.Product.Price;
                var product = _context.Products.Find(item.ProductId);
                product.Stock -= item.Quantity;
                _context.Products.Update(product);
                _context.SaveChanges();
                
            }
            order.TotalAmount = totalAmount;
            _context.Orders.Add(order);
            _context.SaveChanges();


            // Clear the cart after placing the order
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
