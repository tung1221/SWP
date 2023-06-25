using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System.Diagnostics;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopContext _shopContext;
      
        public HomeController(ILogger<HomeController> logger, ShopContext ct)
        {
            _shopContext = ct;
            _logger = logger;
        }

        public IActionResult Index(string mode)
        {
            var listProduct = _shopContext.Products.Where(p => p.HomeStatus == true);

            if (mode != null)
            {
                if (mode.Equals("EDetailProduct"))
                {
                    ViewData["EDetailProduct"] = "Error";
                }
            }

			var user = HttpContext.User;

			if (User.Identity.IsAuthenticated)
			{
				if (User.IsInRole("Admin"))
				{
					ViewBag.ShowAdminButton = true;
				}
				else
				{
					ViewBag.ShowAdminButton = false;
				}

				if (User.IsInRole("Marketing"))
				{
					ViewBag.ShowMarketingButton = true;
				}
				else
				{
					ViewBag.ShowMarketingButton = false;
				}

				if (User.IsInRole("Seller"))
				{
					ViewBag.ShowSellerButton = true;
				}
				else
				{
					ViewBag.ShowSellerButton = false;
				}
			}
			else
			{
				ViewBag.ShowAdminButton = false;
				ViewBag.ShowMarketingButton = false;
				ViewBag.ShowSellerButton = false;
			}


			return View(listProduct.ToList());

            
        }

       
    }
}