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
            List<Bill> bills = _shopContext.Bills.ToList();
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

                return View(bills);
            }
            else
            {
                // Handle the case when the user is not authenticated
                return RedirectToAction("Index");
            }
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


            return RedirectToAction("ViewOrder");
        }

        public IActionResult DetailBill(int billId)
        {
            LoadRoleUser();

            var bill = _shopContext.Bills.FirstOrDefault(b => b.BillId == billId);
            if (bill != null)
            {
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

    }
}
