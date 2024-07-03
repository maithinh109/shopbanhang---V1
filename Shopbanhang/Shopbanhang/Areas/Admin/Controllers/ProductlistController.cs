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
    public class ProductlistController : Controller
    {
        private Entities db = new Entities();

        // GET: Admin/Productlist
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            ViewBag.Cat = new SelectList(db.Categories.ToList(), "CatId", "CatName");
            return View(products.ToList());
        }

        // GET: Admin/Productlist/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Productlist/Create
        public ActionResult Create()
        {
            ViewBag.CatId = new SelectList(db.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Productlist/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.




        // GET: Admin/Productlist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductName,CatId,Price,Image, Quantity")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Invalid model state." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = ex.Message });
            }
        }


        // POST: Admin/Productlist/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductName,Price,Image,Quantity, CatId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.CatId = new SelectList(db.Categories, "CatId", "CatName", product.CatId);


            return View(product);
        }






        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int ProductId)
        {
            try
            {
                Product product = db.Products.Find(ProductId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm để xóa." });
                }

                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log lỗi vào hệ thống hoặc ghi vào cơ sở dữ liệu
                return Json(new { success = false, message = ex.Message });
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
