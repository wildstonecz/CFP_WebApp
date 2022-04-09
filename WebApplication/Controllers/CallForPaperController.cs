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
        public ActionResult Index()
        {
            ViewBag.CallForPaperList = CallForPaper.getFakeCallForPaperList();
            return View();
        }

        public ActionResult Detail(int id) {
            CallForPaper cfp = CallForPaper.getFakeCallForPaperList()[id - 1];
            return View(cfp);
        }
    }
}