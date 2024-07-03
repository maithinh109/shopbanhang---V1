using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Shopbanhang.Models;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Text;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Security.Policy;

namespace Shopbanhang.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private Entities db = new Entities();
        private readonly VnPayLibrary _vnPayLibrary;
        private string vnp_TmnCode = "NKNR7K20"; // Thay bằng mã merchant của bạn
        private string vnp_HashSecret = "6OA083FIA4F86CMKZ0IDUISVFV6C05N7";
        private string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private string vnp_Returnurl = "https://yourwebsite.com/Orders/PaymentCallback"; // URL callback sau khi thanh toán

        public OrdersController()
        {
            _vnPayLibrary = new VnPayLibrary();
        }

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var orders = db.Orders.Include(o => o.AspNetUser)
                                  .Include(o => o.TransactStatu)
                                  .Where(o => o.UserId == userId)
                                  .ToList();
            return View(orders);
        }

        public ActionResult Create()
        {
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;
            ViewBag.giohang = giohang;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Firstname, string Lastname, string Email, string Country, int Phone, string Address, string payment)
        {
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;

            if (payment == "thanh toán bằng vnpay")
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId();
                    var order = new Order
                    {
                        FirstName = Firstname,
                        LastName = Lastname,
                        Email = Email,
                        Country = Country,
                        Phone = Phone,
                        Address = Address,
                        OrderDate = DateTime.Now.Date,
                        TransactStatusId = 1,
                        Deleted = false,
                        Paid = 1,
                        UserId = userId
                    };
                    foreach (var item in giohang)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = order.OrderId;
                        orderDetail.ProductId = item.id;
                        orderDetail.Quantity = item.soluong;
                        orderDetail.Total = item.thanhtien;
                        db.OrderDetails.Add(orderDetail);
                    }
                    db.Orders.Add(order);
                    db.SaveChanges();
                    string orderAmount = (giohang.Sum(x => x.thanhtien) * 100).ToString();                  
                    string vnp_Returnurl = "https://localhost:44322/Orders/PaymentReturn";
                    string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                    string hashSecret = "6OA083FIA4F86CMKZ0IDUISVFV6C05N7";
                    string madonhang = order.OrderId.ToString();

                    _vnPayLibrary.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
                    _vnPayLibrary.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                    _vnPayLibrary.AddRequestData("vnp_TmnCode", "NKNR7K20"); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                    _vnPayLibrary.AddRequestData("vnp_Amount", orderAmount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                    _vnPayLibrary.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
                    _vnPayLibrary.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                    _vnPayLibrary.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                    _vnPayLibrary.AddRequestData("vnp_IpAddr", "192.168.1.1"); //Địa chỉ IP của khách hàng thực hiện giao dịch
                    _vnPayLibrary.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                    _vnPayLibrary.AddRequestData("vnp_OrderInfo", "đơn hàng"); //Thông tin mô tả nội dung thanh toán
                    _vnPayLibrary.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                    _vnPayLibrary.AddRequestData("vnp_ReturnUrl", vnp_Returnurl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                    _vnPayLibrary.AddRequestData("vnp_TxnRef", madonhang); //mã hóa đơn
                    string paymentUrl = _vnPayLibrary.CreateRequestUrl(url, hashSecret);
                    return Redirect(paymentUrl);
                }
                return RedirectToAction("PaymentFail");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId();
                    var order = new Order
                    {
                        FirstName = Firstname,
                        LastName = Lastname,
                        Email = Email,
                        Country = Country,
                        Phone = Phone,
                        Address = Address,
                        OrderDate = DateTime.Now.Date,
                        TransactStatusId = 1,
                        Deleted = false,
                        Paid = 1,
                        UserId = userId
                    };
                    foreach (var item in giohang)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = order.OrderId;
                        orderDetail.ProductId = item.id;
                        orderDetail.Quantity = item.soluong;
                        orderDetail.Total = item.thanhtien;
                        db.OrderDetails.Add(orderDetail);
                    }

                    db.Orders.Add(order);
                    db.SaveChanges();
                    HttpContext.Session.Remove("giohang");

                    return RedirectToAction("OrderPlaced");
                }

                return View();
            }
        }

        public ActionResult OrderPlaced()
        {
            return View();
        }

        public ActionResult ChiTietDonHang(int orderId)
        {
            var orderDetails = db.OrderDetails.Where(od => od.OrderId == orderId).ToList();
            return PartialView("_ChiTietDonHang", orderDetails);
        }

        public ActionResult PaymentFail()
        {
            return View();
        }

        public ActionResult PaymentReturn()
        {
            var vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
            var vnp_TxnRef = Request.QueryString["vnp_TxnRef"];
            var vnp_Amount = Request.QueryString["vnp_Amount"];
            if (vnp_ResponseCode == "00")
            {
                int orderId;
                if (int.TryParse(vnp_TxnRef, out orderId))
                {
                    var order = db.Orders.FirstOrDefault(o => o.OrderId == orderId);
                    if (order != null)
                    {
                        order.Paid = 2; // Chỉ cập nhật khi thanh toán thành công
                        db.SaveChanges();
                        HttpContext.Session.Remove("giohang");
                        return RedirectToAction("OrderPlaced");
                    }
                }




                return RedirectToAction("OrderPlaced");
            }
            return RedirectToAction("PaymentFail");
            
        }
    }
}
