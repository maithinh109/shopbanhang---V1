using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopbanhang.Models;

namespace Shopbanhang.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        // GET: Checkout
        Entities db = new Entities();
        public ActionResult Index()
        {
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;
            ViewBag.Giohang = giohang;
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,CustumerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,note,FirstName,LastName,Country,Address,Phone,Email")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("OrderPlaced");
            }


            ViewBag.CustumerId = new SelectList(db.Customers, "CustumerId", "Fullname", order.CustumerId);
            ViewBag.TransactStatusId = new SelectList(db.TransactStatus, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }
        public ActionResult OrderPlaced()
        {
            return View();
        }
        
    }
}