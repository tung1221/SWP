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

	public class SellerController : Controller
	{
		private readonly ShopContext _shopContext;
		private readonly MailSettings _mailSettings;

		public SellerController(ShopContext shopContext)
		{
			_shopContext = shopContext;
		}


		public IActionResult Index()
		{
			LoadRoleUser();

			return View();
		}

		public IActionResult ViewAll()
		{
			LoadRoleUser();
			List<Bill> bills = _shopContext.Bills.ToList()
				.Where(bill => int.TryParse(bill.BillStatus, out int billStatus) && billStatus < 4)
				.ToList();

			return View(bills);
		}


		public IActionResult ViewOrder()
		{
			LoadRoleUser();
			var currentUser = HttpContext.User;

			if (currentUser.Identity.IsAuthenticated)
			{
				List<Bill> bills = _shopContext.Bills
					.Where(bill => bill.Email == currentUser.Identity.Name)
					.ToList();

				bills = bills.Where(bill => int.TryParse(bill.BillStatus, out int billStatus) && billStatus < 4)
					.ToList();

				return View(bills);
			}
			else
			{
				return RedirectToAction("Index");
			}
		}





		public IActionResult DetailBill(int billId)
		{
			LoadRoleUser();

			var bill = _shopContext.Bills.FirstOrDefault(b => b.BillId == billId);
			if (bill != null)
			{
				if (TempData.ContainsKey("OutOfStockFlag"))
				{
					ViewBag.OutOfStockMessage = "Hàng đã hết. Bạn có muốn xóa không?";
					TempData.Remove("OutOfStockFlag");
				}

				return View(bill);

			}
			else
			{
				return Redirect("Index");
			}

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

		public IActionResult Delete(int billId)
		{
			var bill = _shopContext.Bills.Include(b => b.BillDetails).FirstOrDefault(b => b.BillId == billId);
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
                                        <h1>Đơn hàng của bạn đã bị hủy do gặp vấn đề. Chúng tôi vô cùng xin lỗi về vấn đề này và sẽ sớm hoàn lại tiền cho bạn.</h1>
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
					// Xóa tất cả các đối tượng BillDetail liên quan
					_shopContext.BillDetails.RemoveRange(bill.BillDetails);
					_shopContext.Bills.Remove(bill);
					_shopContext.SaveChanges();
				}
				else
				{
					// Xử lý khi gửi email thất bại
					// ...
				}

			}

			return RedirectToAction("ViewAll", "Seller");

		}
		public IActionResult ProcessBill(int billId)
		{


			var bill = _shopContext.Bills
		.Include(b => b.BillDetails)
			.ThenInclude(bd => bd.Product)
				.ThenInclude(p => p.ProductDetails)
		.FirstOrDefault(b => b.BillId == billId);

			if (bill != null)
			{
				if (bill.BillStatus == "0")
				{
					foreach (var billDetail in bill.BillDetails)
					{
						var product = billDetail.Product;
						var productDetail = product.ProductDetails
							.FirstOrDefault(pd => pd.color == billDetail.color && pd.size == billDetail.size);

						if (productDetail != null && productDetail.quantity >= billDetail.quantity)
						{

							productDetail.quantity -= billDetail.quantity;


							_shopContext.SaveChanges();
						}
						else
						{
							TempData["OutOfStockFlag"] = true;
							return RedirectToAction("DetailBill", new { billId });
						}
					}

					int billStatus = int.Parse(bill.BillStatus) + 1;
					bill.BillStatus = billStatus.ToString();
					_shopContext.SaveChanges();

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
					if (sendResult != "gui email thanh cong")
					{
						// Handle email sending failure
						// ...
					}
				}
				else
				{
					int billStatus = int.Parse(bill.BillStatus) + 1;
					bill.BillStatus = billStatus.ToString();
					_shopContext.SaveChanges();
				}
			}


			return RedirectToAction("ViewOrder");
		}
		public IActionResult ProcessBillAll(int billId)
		{


			var bill = _shopContext.Bills
		.Include(b => b.BillDetails)
			.ThenInclude(bd => bd.Product)
				.ThenInclude(p => p.ProductDetails)
		.FirstOrDefault(b => b.BillId == billId);

			if (bill != null)
			{
				if (bill.BillStatus == "0")
				{
					foreach (var billDetail in bill.BillDetails)
					{
						var product = billDetail.Product;
						var productDetail = product.ProductDetails
							.FirstOrDefault(pd => pd.color == billDetail.color && pd.size == billDetail.size);

						if (productDetail != null && productDetail.quantity >= billDetail.quantity)
						{

							productDetail.quantity -= billDetail.quantity;


							_shopContext.SaveChanges();
						}
						else
						{
							TempData["OutOfStockFlag"] = true;
							return RedirectToAction("DetailBill", new { billId });
						}
					}

					int billStatus = int.Parse(bill.BillStatus) + 1;
					bill.BillStatus = billStatus.ToString();
					_shopContext.SaveChanges();

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
					if (sendResult != "gui email thanh cong")
					{
						// Handle email sending failure
						// ...
					}
				}
				else
				{
					int billStatus = int.Parse(bill.BillStatus) + 1;
					bill.BillStatus = billStatus.ToString();
					_shopContext.SaveChanges();
				}
			}



			return RedirectToAction("ViewAll");
		}


	}
}
