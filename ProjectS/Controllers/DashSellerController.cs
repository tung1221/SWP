using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
	public class DashSellerController : Controller
	{
		private readonly ShopContext _shopContext;
		public DashSellerController(ShopContext shopContext)
		{
			_shopContext = shopContext;
		}


		public IActionResult Index()
		{

            List<Bill> bills = _shopContext.Bills.Include(b => b.User).ToList();
            return View(bills);
		}

		public IActionResult ProcessBill(int billId)
		{
			

				var bill = _shopContext.Bills.FirstOrDefault(b => b.BillId == billId);

				if (bill != null)
				{
					int billParse = int.Parse(bill.BillStatus) +1;


					bill.BillStatus = $"{billParse}";

					_shopContext.SaveChanges();
				}

			
			return RedirectToAction("Index");
		}
	}
}
