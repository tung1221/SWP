using Microsoft.AspNetCore.Mvc;
using Project.Data;
using WebApplication6.Service;

namespace Project.Controllers
{
    public class AdminController : Controller
    {
		private readonly ShopContext _shopContext;
        private readonly ICloudinaryService _cloudinaryService;
        public AdminController(ShopContext shopContext, ICloudinaryService temp)
		{
			_shopContext = shopContext;
            _cloudinaryService = temp;
        }
		public IActionResult Index()
        {
			LoadRoleUser();
			return View();
        }

       
        private void LoadRoleUser()
		{
			var user = HttpContext.User;

			if (user.Identity.IsAuthenticated)
			{
				if (user.IsInRole("Admin"))
				{
					ViewBag.ShowAdminButton = true;
				}
				else
				{
					ViewBag.ShowAdminButton = false;
				}

				if (user.IsInRole("Marketing"))
				{
					ViewBag.ShowMarketingButton = true;
				}
				else
				{
					ViewBag.ShowMarketingButton = false;
				}

				if (user.IsInRole("Seller"))
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
		}

	}
}
