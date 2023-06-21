using Microsoft.AspNetCore.Mvc;
using Project.Data;

namespace Project.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopContext _shopContext;
        public CartController(ILogger<HomeController> logger, ShopContext ct)
        {
            _shopContext = ct;
            _logger = logger;
        }

        public IActionResult Index(IFormCollection f)
        {
            ViewData["color"] = f["color"];
            ViewData["size"] = f["size"];
            ViewData["Quantity"] = f["Quantity"];
            return View();
        }
     
    }
}
