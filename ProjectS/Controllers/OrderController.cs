using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            ViewData["list"] = f["list"];
            ViewData["total"] = f["total"];
            string listCartItemId = f["list"];
            List<Product> products = new List<Product>();
            List<int> quantity = new List<int>();


            char[] temp = { ' ', ',' };
            string[] str1 = listCartItemId.Split(temp, StringSplitOptions.RemoveEmptyEntries);

            

            var total = f["total"];

            return View();
        }

        public IActionResult ProcessOrder(IFormCollection f)
        {
            ViewData["list"] = f["list"];

            var total = f["total"];
            Bill temp = new Models.Bill() { UserId = "bffd4951-7d06-4f94-9848-63297a8f838c", TransportId = 1, BillStatus = "0", PaymentCode = 1263272, PurchaseDate = DateTime.Now, PaymentMethod = "momo", ShippingAddress = "", ShippingFee = 0, TotalPrice = double.Parse(total) };
           _shopContext.Bills.Add(temp);


            _shopContext.SaveChanges();

            ViewData["d"] = temp.BillId;

            return View();
        }
    }
}
