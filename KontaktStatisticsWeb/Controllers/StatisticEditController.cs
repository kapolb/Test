using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kontakt.BL;
using System.Configuration;
using KontaktStatisticsWeb.Models;
using Kontakt.DAL;
using System.IO;

namespace KontaktStatisticsWeb.Controllers
{
    public class StatisticEditController : Controller
    {
        StatisticsDataFactory _factory = null;

        StatisticsDataFactory Factory
        {
            get
            {
                if (this._factory == null)
                {
                    this._factory = new StatisticsDataFactory(
                        ConfigurationManager.ConnectionStrings["KontaktDB2011"].ConnectionString,
                        ConfigurationManager.AppSettings["TemplatesDirectory"] + "Statistics.xml");
                }
                return this._factory;
            }
        }
        public ActionResult Index(int id)
        {
            StatisticInfo info = null;
            if (id > 0)
                info = this.Factory.AllStatistics.FirstOrDefault(item => item.Id.Equals(id));
            if (info == null)
                info = new StatisticInfo();
            StatInfoModel model = new StatInfoModel(info);
            this.FillModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(StatInfoModel model)
        {
            this.FillModel(model);
            if (this.ViewData.ModelState.IsValid)
            {
                this.Factory.UpdateStatisticInfo(model.StatInfo);
                return RedirectToAction("Index", "He");
            }
            return View(model);
        }

        private void FillModel(StatInfoModel model)
        {
            StatisticsDBHelper helper = new StatisticsDBHelper();
            helper.AddConnection("2011", ConfigurationManager.ConnectionStrings["KontaktDB2011"].ConnectionString, 2011, true);
            model.FiltersForData = helper.GetFilters(55);
            model.FiltersForData.Insert(0, new Kontakt.DataModel.IdValue() { Id = -1, Value = " --- Vyber hodnotu --- " });
            model.FiltersForOrdered = helper.GetFilters(12);
            model.FiltersForOrdered.Insert(0, new Kontakt.DataModel.IdValue() { Id = -1, Value = " --- Vyber hodnotu --- " });
            model.FiltersForOnStock = helper.GetFilters(20);
            model.FiltersForOnStock.Insert(0, new Kontakt.DataModel.IdValue() { Id = -1, Value = " --- Vyber hodnotu --- " });
            model.AllTemplates = this.GetFileNames(ConfigurationManager.AppSettings["TemplatesDirectory"], "*.xls");
            model.AllUsages = this.GetFileNames(ConfigurationManager.AppSettings["UsagesDirectory"], "*.xml");
            model.AllUsages.Insert(0, "");

            model.StatInfo.FilterForData = this.FillValue(model.FiltersForData, model.StatInfo.FilterForDataId);
            model.StatInfo.FilterForOnStock = this.FillValue(model.FiltersForOnStock, model.StatInfo.FilterForOnStockId);
            model.StatInfo.FilterForOrdered = this.FillValue(model.FiltersForOrdered, model.StatInfo.FilterForOrderedId);
        }

        private string FillValue(IList<Kontakt.DataModel.IdValue> coll, int id)
        {
            if (id > 0)
            {
                Kontakt.DataModel.IdValue value = coll.FirstOrDefault(it => it.Id == id);
                if (value != null)
                    return value.Value;
            }
            return null;
        }

        private IList<string> GetFileNames(string path, string searchPattern)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, searchPattern);
                List<string> names = new List<string>();
                foreach (string file in files)
                    names.Add(Path.GetFileNameWithoutExtension(file));
                return names;
            }
            return new List<string>();
        }
    }
}
