using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityCFP;

namespace WebApplication.Controllers
{
    public class CallForPaperController : Controller
    {
        // GET: CallForPaper
        public ActionResult Index() {
            ViewBag.CallForPaperList = CallForPaper.getCallForPaperList();
            return View();
        }

        public ActionResult Detail(int id) {
            ViewBag.Title = "Detail";
            CallForPaper cfp = CallForPaper.getCallForPaper(id);
            return View(cfp);
        }
    }
}