using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityCFP;

namespace WebApplication.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            ViewBag.CategoryList = Category.getCategoryList();
            return View();
        }

        public ActionResult Detail(int id) {
            ViewBag.Title = "Detail";
            Category category = Category.getCategoryByID(id);
            return View(category);
        }
    }
}