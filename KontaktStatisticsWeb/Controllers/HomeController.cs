using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kontakt.BL;
using System.Configuration;
using KontaktStatisticsWeb.Models;

namespace KontaktStatisticsWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Vyberte požadovaný dokument.";

            return View();
        }

        public ActionResult About()
        {
            StatisticDefinition definition = new StatisticDefinition();
            return View(definition);
        }

        [HttpPost]
        public ActionResult About(StatisticDefinition definition)
        {
            //StatisticDefinition definition = null;
            if (definition == null)
                definition = new StatisticDefinition();
            return View(definition);
        }
    }
}
