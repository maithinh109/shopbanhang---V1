using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shopbanhang.Models;

namespace Shopbanhang.Areas.Admin.Controllers
{
    public class OrdersAdminController : Controller
    {
        private Entities db = new Entities();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Customer).Include(o => o.TransactStatu);
            ViewBag.TransactStatusList = new SelectList(db.TransactStatus, "TransactStatusId", "Status");
           
            return View(orders.ToList());
        }

        // GET: Admin/Orders/Details/5
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var chitietdonhang = db.OrderDetails.Where(x => x.OrderId == order.OrderId).OrderBy(x => x.OrderDetaild).ToList();
            return Json(new { success = true, data = chitietdonhang }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChiTietDonHang(int orderId)
        {
            var orderDetails = db.OrderDetails.Where(od => od.OrderId == orderId).ToList();
            return PartialView("_ChiTietDonHang", orderDetails);
        }





        [HttpPost]

        public ActionResult Delete(int orderId)
        {
            Order order = db.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }
            var orderdetails = db.OrderDetails.Where(x => x.OrderId == orderId).ToList();
            foreach (var item in orderdetails)
            {
                db.OrderDetails.Remove(item);
            }
            db.Orders.Remove(order);
            db.SaveChanges();
            return Json(new { success = true });
        }




        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, int statusId)
        {
            var order = db.Orders.Find(orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            order.TransactStatusId = statusId;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}
