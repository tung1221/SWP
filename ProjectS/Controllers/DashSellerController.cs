using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Service;
using System.Data;

namespace Project.Controllers
{
	[Authorize(Roles = "Seller")]

	public class DashSellerController : Controller
	{
		private readonly ShopContext _shopContext;
		public DashSellerController(ShopContext shopContext)
		{
			_shopContext = shopContext;
		}


		public IActionResult Index()
		{
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


			List<Bill> bills = _shopContext.Bills.Include(b => b.User).ToList();
            return View(bills);
		}

		public IActionResult ProcessBill(int billId)
		{
			
				
				var bill = _shopContext.Bills.FirstOrDefault(b => b.BillId == billId);
				

				if (bill != null)
				{
					
					string fromEmail = "huongdl40@gmail.com";
					string toEmail = bill.Email;
					string subject = "Xác nhận đơn hàng";
					string body = @"
                                    <html>
                                    <head>
                                        <style>
                                            
                                        </style>
                                    </head>
                                    <body>
                                        <h1>Đơn hàng của bạn đã được xác nhận.</h1>
                                        <p>Thông tin đơn hàng:</p>
                                        <ul>
                                            <li>Tên người dùng: " + bill.Email + @"</li>                                                                                     
                                             <li>Tổng giá tiền: " + bill.TotalPrice + @".000vnd</li>
                                        </ul>
                                    </body>
                                    </html>";
                string gmail = "huongdl40@gmail.com";
				string password = "gepcdegcpjjzceke";
                var sendResult = SendMailConfirmOrder.SendGmail(fromEmail, toEmail, subject, body, gmail, password).GetAwaiter().GetResult();
                if (sendResult == "gui email thanh cong")
                {
                    int billParse = int.Parse(bill.BillStatus) + 1;
                    bill.BillStatus = $"{billParse}";
                    _shopContext.SaveChanges();
                }
                else
                {
                    // Xử lý khi gửi email thất bại
                    // ...
                }

				}

			
			return RedirectToAction("Index");
		}
	}
}
