using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Macs;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ShopContext _shopContext;
        private readonly SignInManager<IdentityUser> _SignInManager;
        public OrderController(ILogger<OrderController> logger, ShopContext ct, SignInManager<IdentityUser> SignInManager)
        {
            _shopContext = ct;
            _logger = logger;
            _SignInManager = SignInManager;
        }

        public IActionResult Index(IFormCollection f)
        {
            var products = _shopContext.Products.ToList();
            double? total = 0;


            foreach (var cartItem in getListItem())
            {
                foreach(var product in products)
                {
                    if(product.ProductId == cartItem.ProductId)
                    {
                        total = total + cartItem.Quantity* ( product.ProductPrice - product.Discount );
                    }
                }
            }

            ViewData["products"] = products;
            ViewData["total"] = total;
            return View(getListItem());
        }

        [HttpPost]
        public IActionResult ProcessOrder(IFormCollection f)
        {
            string email = f["email"];
            var total = f["total"];

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Order");
            }
            Bill temp1 = new Models.Bill() { Email = email, UserId = "bffd4951-7d06-4f94-9848-63297a8f838c", TransportId = 1, BillStatus = "0", PaymentCode = 1263272, PurchaseDate = DateTime.Now, PaymentMethod = "momo", ShippingAddress = "", ShippingFee = 0, TotalPrice = double.Parse(total) };
            _shopContext.Bills.Add(temp1);
            _shopContext.SaveChanges();

            foreach (var cartItem in getListItem())
            {
                _shopContext.BillDetails.Add(new BillDetail() { BillId = temp1.BillId, ProductId = cartItem.ProductId, quantity = cartItem.Quantity });
            }

            _shopContext.SaveChanges();
            return View();
        }
        public IActionResult ProcessOrder()
        {
            return View();
        }

        public List<CartItem> getListItem()
        {
            List<CartItem> cartItems = new List<CartItem>();

            if (_SignInManager.IsSignedIn(User))
            {
                string userId = _SignInManager.UserManager.GetUserId(User);
                int cartId = (from c in _shopContext.Carts
                              where c.UserId == userId
                              select c.CartId).FirstOrDefault();
                cartItems = _shopContext.CartItems.Include(i => i.product).Where(i => i.CartId == cartId).ToList();
            }

            string? cookieValue = Request.Cookies["cart"];
            if (cookieValue != null)
            {
                List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                cartItems.AddRange(cartListFromCookie);
            }
            cartItems = cartItems.OrderByDescending(p => p.CartItemId).ToList();

            return cartItems;
        }
    }
}
