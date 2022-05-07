using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityCFP;

namespace WebApplication.Controllers
{
    public class SeriesController : Controller
    {
        // GET: Series
        public ActionResult Index()
        {
            ViewBag.CallForPaperSeriesList = CallForPaperSeries.getCallForPaperSeriesList();
            return View();
        }

        public ActionResult Detail(int id) {
            ViewBag.Title = "Detail";
            CallForPaperSeries serie = CallForPaperSeries.getCallForPaperSeries(id);
            return View(serie);
        }
    }
}