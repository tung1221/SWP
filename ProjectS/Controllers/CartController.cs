using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System.Drawing;
using System.Security.Cryptography;

namespace Project.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopContext _shopContext;
        private readonly SignInManager<IdentityUser> _SignInManager;
        public CartController(ILogger<HomeController> logger, ShopContext ct, SignInManager<IdentityUser> SignInManager)
        {
            _shopContext = ct;
            _logger = logger;
            _SignInManager = SignInManager;
        }

        public IActionResult Index(string choice, int id)
        {
            List<CartItem> cartItems = new List<CartItem>();

            if (choice.Equals("remove"))
            {
                if (_SignInManager.IsSignedIn(User))
                {
                    string userId = _SignInManager.UserManager.GetUserId(User);
                    int cartId = (from c in _shopContext.Carts
                                  where c.UserId == userId
                                  select c.CartId).FirstOrDefault();
                    var item = _shopContext.CartItems.Find(id);
                    if (item != null)
                    {
                        _shopContext.CartItems.Remove(item);
                        _shopContext.SaveChanges();
                    }
                    _shopContext.SaveChanges();
                    cartItems = _shopContext.CartItems.Include(i => i.product).Where(i => i.CartId == cartId).OrderByDescending(i => i.CartItemId).ToList();
                }
            }

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Index(IFormCollection f)
        {

            var color = f["color"];
            var size = f["size"];
            var qua = int.Parse(f["Quantity"]);
            int pId = int.Parse(f["productId"]);
            List<CartItem> cartItems = new List<CartItem>();
            if (_SignInManager.IsSignedIn(User))
            {
                string userId = _SignInManager.UserManager.GetUserId(User);
                int cartId = (from c in _shopContext.Carts
                              where c.UserId == userId
                              select c.CartId).FirstOrDefault();

                _shopContext.CartItems.Add(new Models.CartItem() { CartId = cartId, ProductId = pId, color = color, Quantity = qua, size = size });
                _shopContext.SaveChanges();

                cartItems = _shopContext.CartItems.Include(i => i.product).Where(i => i.CartId == cartId).OrderByDescending(i => i.CartItemId).ToList();
            }

            return View(cartItems);
        }



    }
}
