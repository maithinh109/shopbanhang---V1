using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shopbanhang.Models;
using PagedList;

namespace Shopbanhang.Controllers
{
    public class ProductsController : Controller
    {
        private Entities db = new Entities();

        
        public ActionResult Index(int? page, string search, int categoryid = 0)
        {
            //tim kiem
            var product = db.Products.Include(p => p.Category);
            if (search != null)
            {
                search = search.ToLower();
                product = product.Where(p => p.ProductName.ToLower().Contains(search));

            }
            // loc theo danh muc
            if (categoryid != 0)
            {
                product = product.Where(p=>p.CatId == categoryid);

            }
           
            

            //phan trang
            var PageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 8;
            var sortedProducts = product.OrderByDescending(x => x.ProductName);

            var model = new PagedList<Product>(sortedProducts, PageNumber, pageSize);
            ViewBag.CurrentPage = PageNumber;
            ViewBag.keysearch = search;
            ViewBag.CategoryID = new SelectList(db.Categories, "CatId", "CatName"); 
            return View(model);
        }

        // GET: Products/Details/5
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

        
       
      


       

        
     

       
    }
}
