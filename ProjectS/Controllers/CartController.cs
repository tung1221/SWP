using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Project.Data;
using Project.Models;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Policy;

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



            if (string.IsNullOrEmpty(choice) || id == 0)
            {
                cartItems = getListItem();

            }
            else
            {
                if (choice.Equals("remove"))
                {
                    if (id >= 0)
                    {
                        var item = _shopContext.CartItems.Find(id);
                        if (item != null)
                        {
                            _shopContext.CartItems.Remove(item);
                            _shopContext.SaveChanges();
                        }
                        _shopContext.SaveChanges();
                        cartItems = getListItem();
                    }
                    else
                    {
                        string? cookieValue = Request.Cookies["cart"];
                        if (cookieValue != null)
                        {
                            cartItems = getListItem();
                            List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                            var cartItem = cartListFromCookie.Find(p => p.CartItemId == id);
                            if (cartItem != null)
                            {
                                cartListFromCookie.Remove(cartItem);
                                cartItems.Remove(cartItems.Find(p => p.CartItemId == id));
                            }
                            if (cartListFromCookie.Count == 0 || cartListFromCookie == null)
                            {
                                Response.Cookies.Delete("cart");
                            }
                            else
                            {
                                string json = JsonConvert.SerializeObject(cartListFromCookie);
                                var option = new CookieOptions()
                                {
                                    Expires = DateTime.Now.AddDays(90)
                                };
                                Response.Cookies.Append("cart", json, option);
                            }
                        }
                    }
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
            }
            else
            {
                string? cookieValue = Request.Cookies["cart"];
                if (!string.IsNullOrEmpty(cookieValue))
                {
                    List<CartItem> cartListFromCookie = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue);
                    if (cartListFromCookie.Count != 0)
                    {
                        cartListFromCookie = cartListFromCookie.OrderBy(p => p.CartItemId).ToList();
                        var cartItemId = cartListFromCookie[cartListFromCookie.Count - 1].CartItemId;
                        if (cartItemId == 0)
                        {
                            Response.Cookies.Delete("cart");
                            return RedirectToAction("Index", "Home");
                        }

                        cartListFromCookie.Add(new Models.CartItem() { CartItemId = cartItemId + 1, ProductId = pId, color = color, Quantity = qua, size = size });
                        cartItems.Add((new Models.CartItem() { CartItemId = cartItemId + 1, ProductId = pId, color = color, Quantity = qua, size = size }));

                        string json = JsonConvert.SerializeObject(cartListFromCookie);
                        var option = new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(90)
                        };
                        Response.Cookies.Append("cart", json, option);
                    }
                }
                else
                {
                    CartItem cartItem = new Models.CartItem() { CartItemId = -999999999, ProductId = pId, color = color, Quantity = qua, size = size };
                    List<CartItem> saveCookie = new List<CartItem>() { cartItem };
                    string json = JsonConvert.SerializeObject(saveCookie);

                    var option = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddDays(90)
                    };

                    Response.Cookies.Append("cart", json, option);
                    cartItems.Add(cartItem);
                }
            }
            cartItems.AddRange(getListItem());
            return View(cartItems);
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
                var temp = _shopContext.CartItems.Include(i => i.product).Where(i => i.CartId == cartId);


                if (temp != null)
                {
                    cartItems = temp.ToList();
                }
               
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
