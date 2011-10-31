using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kontakt.BL;
using System.Configuration;
using KontaktStatisticsWeb.Models;
using Kontakt.DataModel;

namespace KontaktStatisticsWeb.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Index()
        {
            StatisticsDataFactory factory = new StatisticsDataFactory(
                ConfigurationManager.ConnectionStrings["KontaktDB2011"].ConnectionString,
                ConfigurationManager.AppSettings["TemplatesDirectory"] + "Statistics.xml",
                ConfigurationManager.AppSettings["UsagesDirectory"]);
            StatisticDefinition definition = new StatisticDefinition();
            StatisticDefinition.All = factory.AllStatistics;
            StatisticDefinition.All.Insert(0, new StatisticInfo() { Id = 0, Name = "-- Nieco vyber --" });
            return View(definition);
        }

        [HttpPost]
        public ActionResult Index(StatisticDefinition definition)
        {
            if (definition.Id > 0)
            {
                //string he = System.IO.Directory.GetCurrentDirectory();
                string templatesDirectory = ConfigurationManager.AppSettings["TemplatesDirectory"];
                ExcelBL.ExcelManager manager = ExcelBL.ExcelManager.CreateManagerWithTemplate(templatesDirectory);
                StatisticsDataFactory factory = new StatisticsDataFactory(
                    ConfigurationManager.ConnectionStrings["KontaktDB2011"].ConnectionString,
                    templatesDirectory + "Statistics.xml",
                    ConfigurationManager.AppSettings["UsagesDirectory"]);
                Items its = factory.GetStatisticsData(definition.Id);
                //List<ItemData> data = new List<ItemData>(factory.GetStatisticsData(definition.Id).Rows);
                StatisticInfo info = factory.AllStatistics.FirstOrDefault(item => item.Id.Equals(definition.Id));
                if (info == null)
                    return View(definition);
                //string generationDirectory = ConfigurationManager.AppSettings["GenerationDirectory"];
                Guid guid = Guid.NewGuid();
                //string filename = string.Format("{0}{1}.xls", generationDirectory, guid);
                byte[] content = manager.GenerateReportContent(info.TemplateName, its);
                // ulozime si potrebne info -> zatial takto, mozno najdem aj inu moznost
                return this.File(content, "application/x-ms-excel", string.Format("{0}.xls", info.GenerateName));
                TempData["FileContent"] = content;
                TempData["StatisticName"] = info.GenerateName;
                // A redirectnem to na download 
                // - musi to byt urobene tymto sposobom, lebo ak som to posielal priamo, 
                // bloklo mi to dropdown s vyberom statistiky
                return this.RedirectToAction("Download");
            }
            return View(definition);
        }
        public ActionResult Download()
        {
            // nacitam si potrebne info
            byte[] content = (byte[])TempData["FileContent"];
            TempData["FileContent"] = null;
            string name = TempData["StatisticName"].ToString();
            TempData["StatisticName"] = null;
            // A tutoka poslem subor na klienta
            if (content != null && content.Length > 0)
                return this.File(content, "application/x-ms-excel", string.Format("{0}.xls", !string.IsNullOrWhiteSpace(name) ? name : "Default"));
            return RedirectToAction("Index");
        }
    }
}
