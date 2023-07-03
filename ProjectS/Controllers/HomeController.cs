using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            ViewData["listBlog"] = _shopContext.Blogs.Include(p => p.ImageBlogs).Where(p => p.HomeStatus == true).OrderBy(p => p.Blogid).ToList();
            return View(listProduct.ToList());


        }


    }
}