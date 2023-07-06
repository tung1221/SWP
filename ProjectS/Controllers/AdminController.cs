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
            DateTime now = DateTime.Now;
            var x = _shopContext.Bills.Where(p => p.PurchaseDate.Year == now.Year).Where(p => p.BillStatus.Equals("3")).ToList();
            double total = 0;
            double totalMonth = 0;
            double totalDay = 0;
            SortedDictionary<int, double> myDictionary = new SortedDictionary<int, double>();

            foreach (var l in x)
            {
                if (myDictionary.ContainsKey(l.PurchaseDate.Month))
                {
                    myDictionary[l.PurchaseDate.Month] += l.TotalPrice;
                }
                else
                {
                    myDictionary.Add(l.PurchaseDate.Month, l.TotalPrice);
                }
                total += l.TotalPrice;
                if (l.PurchaseDate.Month == now.Month)
                {
                    totalMonth += l.TotalPrice;
                    if (l.PurchaseDate.Day == now.Day)
                        totalDay += l.TotalPrice;
                }
            }

            ViewData["total"] = total;
            ViewData["totalMonth"] = totalMonth;
            ViewData["totaday"] = totalDay;
            return View(myDictionary);
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
