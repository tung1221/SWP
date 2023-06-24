using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Service;

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
			string userRoles = HttpContext.Session.GetString("UserRoles");
			if (!string.IsNullOrEmpty(userRoles))
			{
				List<string> roles = userRoles.Split(',').ToList();
				ViewBag.ShowAdminButton = roles.Contains("Admin");
				ViewBag.ShowMarketingButton = roles.Contains("Marketing");
				ViewBag.ShowSellerButton = roles.Contains("Seller");
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
