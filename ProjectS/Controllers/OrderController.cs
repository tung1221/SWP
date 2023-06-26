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
                foreach (var product in products)
                {
                    if (product.ProductId == cartItem.ProductId)
                    {
                        total = total + cartItem.Quantity * (product.ProductPrice - product.Discount);
                    }
                }
            }

            ViewData["products"] = products;
            ViewData["total"] = total;
            return View(getListItem());
        }

        [HttpPost]
        public IActionResult ProcessOrder(string email, string total)
        {
            _logger.LogError("yyyyyyyy");
            _logger.LogError("dendayx" + email);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("email: " + email);
                return RedirectToAction("Index", "Order");
            }

            var RoldId = _shopContext.Roles.Where(c => c.NormalizedName == "SELLER").ToList().FirstOrDefault();
            List<string> lSellerId = _shopContext.UserRoles.Where(c => c.RoleId == RoldId.Id).Select(c => c.UserId).ToList();
            Dictionary<string, int> myDictionary = new Dictionary<string, int>();
            foreach (var selletId  in lSellerId)
            {
                myDictionary.Add(selletId, 0);
            }

            foreach(var b in _shopContext.Bills.Where(c => c.BillStatus == "0").Select(c => c.sellerId).ToList())
            {
                var pair = myDictionary.FirstOrDefault(x => x.Key == b);
                myDictionary[pair.Key] = myDictionary[pair.Key] + 1;
            }

            var minPair = myDictionary.MinBy(pair => pair.Value);



            Bill temp1 = new Models.Bill() { sellerId = minPair.Key, Email = email, UserId = "bffd4951-7d06-4f94-9848-63297a8f838c", TransportId = 1, BillStatus = "0", PaymentCode = 1263272, PurchaseDate = DateTime.Now, PaymentMethod = "momo", ShippingAddress = "", ShippingFee = 0, TotalPrice = double.Parse(total) };
            _shopContext.Bills.Add(temp1);
            _shopContext.SaveChanges();

            foreach (var cartItem in getListItem())
            {
                _shopContext.BillDetails.Add(new BillDetail() { BillId = temp1.BillId, ProductId = cartItem.ProductId, quantity = cartItem.Quantity });
                if(cartItem.CartItemId >= 0)
                {
                    _shopContext.CartItems.Remove(cartItem);
                }
            }

            _shopContext.SaveChanges();
            Response.Cookies.Delete("cart");
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
