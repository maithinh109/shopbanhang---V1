using Shopbanhang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopbanhang.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeadminController : Controller
    {
        // GET: Admin/Homeadmin
        private Entities db = new Entities();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetSalesData()
        {
            var salesData = db.OrderDetails
                .GroupBy(od => od.Product)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                }).ToList();

            return Json(salesData, JsonRequestBehavior.AllowGet);
        }
    }
}