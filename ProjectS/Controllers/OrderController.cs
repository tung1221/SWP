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
            var total = f["total"];

            return View();
        }

        public IActionResult ProcessOrder(IFormCollection f)
        {
            ViewData["list"] = f["list"];

            var total = f["total"];
           
                var bill = _shopContext.Bills.Add(new Models.Bill() { UserId = "bffd4951-7d06-4f94-9848-63297a8f838c", TransportId = 1, BillStatus = "0", PaymentCode = 1263272, PurchaseDate = DateTime.Now, PaymentMethod = "momo", ShippingAddress = "", ShippingFee = 0, TotalPrice = double.Parse(total) });

           


            _shopContext.SaveChanges();



            return View();
        }
    }
}
