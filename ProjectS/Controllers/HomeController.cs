using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System.Diagnostics;
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

        public IActionResult Index()
        {
            var temp = _shopContext.Products.Where(p => p.HomeStatus == true);

            return View(temp.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}