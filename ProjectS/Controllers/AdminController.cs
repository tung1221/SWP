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

		public IActionResult cfFeedback()
		{
			var feedbacks = _shopContext.Feedbacks.ToList();

			LoadRoleUser();
			return View(feedbacks);
		}

		public IActionResult confirmFeedback(int feedbackId)
		{
			var feedback = _shopContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
			if (feedback != null)
			{
				feedback.FeedbackStatus = "1";
				_shopContext.SaveChanges();
			}

			return RedirectToAction("cfFeedback", "admin");
		}
		public IActionResult deleteFb(int feedbackId)
		{
			var feedback = _shopContext.Feedbacks.FirstOrDefault(f => f.FeedbackId == feedbackId);
			if (feedback != null)
			{
				_shopContext.Feedbacks.Remove(feedback);
				_shopContext.SaveChanges();
			}
			return RedirectToAction("cfFeedback", "admin");
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
