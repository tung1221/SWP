﻿using Microsoft.AspNetCore.Identity;
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


            // So sánh session để tạo nút chuyển hướng
            string userRoles = HttpContext.Session.GetString("UserRoles");
            if (!string.IsNullOrEmpty(userRoles))
            {
                List<string> roles = userRoles.Split(',').ToList();
                ViewBag.ShowAdminButton = roles.Contains("Admin");
                ViewBag.ShowMarketingButton = roles.Contains("Marketing");
                ViewBag.ShowSellerButton = roles.Contains("Seller");
            }


            return View(listProduct.ToList());

            
        }

       
    }
}