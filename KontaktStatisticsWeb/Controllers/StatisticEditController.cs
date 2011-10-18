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
    public class StatisticEditController : Controller
    {
        //
        // GET: /StatisticEdit/

        public ActionResult Index()
        {
            StatisticsDataFactory factory = new StatisticsDataFactory(
                ConfigurationManager.ConnectionStrings["KontaktDB2011"].ConnectionString,
                ConfigurationManager.AppSettings["TemplatesDirectory"] + "Statistics.xml");
            Settings settings = new Settings();
            settings.All = factory.AllStatistics;
            return View(settings);
        }
    }
}
