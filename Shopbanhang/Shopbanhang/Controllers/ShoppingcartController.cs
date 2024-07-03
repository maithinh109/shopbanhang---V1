using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shopbanhang.Models;

namespace Shopbanhang.Controllers
{
    public class ShoppingcartController : Controller
    {
        Entities db = new Entities();


        public ActionResult addtocart(int ProductId, int soluong)
        {
            Product sanpham = db.Products.Find(ProductId);
            if (sanpham.Quantity < soluong)
            {
                TempData["ErrorMessage"] = "Sản phẩm không đủ số lượng!";
                return RedirectToAction("Index", "Products"); // Hoặc chuyển hướng đến trang khác
            }

            if (Session["giohang"] == null)
            {
                Session["giohang"] = new List<cartitem>();
            }
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;
            if (giohang.FirstOrDefault(m => m.id == ProductId) == null)
            {

                cartitem item = new cartitem
                {
                    id = ProductId,
                    name = sanpham.ProductName,
                    price = sanpham.Price.Value,
                    soluong = soluong,
                    img = sanpham.Image

                };
                sanpham.Quantity = sanpham.Quantity - soluong;
                db.SaveChanges();
                giohang.Add(item);
            }
            else
            {
                cartitem item = giohang.FirstOrDefault(p => p.id == ProductId);

                item.soluong += soluong;
                sanpham.Quantity = sanpham.Quantity - soluong;
                db.SaveChanges();
            }
            HttpContext.Session.Add("giohang", giohang);
            ViewBag.SoLuongSP = HttpContext.Session["soluongsp"] ?? 0;

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;
            return View(giohang);

        }
        public ActionResult suasoluong(int id, int soluongmoi)
        {
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;
            cartitem item = giohang.FirstOrDefault(m => m.id == id);
            if (item != null)
            {
                item.soluong = soluongmoi;
            }
            return RedirectToAction("index");



        }
        [HttpPost]

        public ActionResult XoaKhoiGio(int Id, int soluong)
        {
            List<cartitem> giohang = Session["giohang"] as List<cartitem>;
            Product sanpham = db.Products.Find(Id);
            cartitem itemXoa = giohang.FirstOrDefault(m => m.id == Id);
            if (itemXoa != null)
            {
                sanpham.Quantity = sanpham.Quantity + soluong;
                giohang.Remove(itemXoa);
            }
            return Json(new { success = true });

        }
    }
}