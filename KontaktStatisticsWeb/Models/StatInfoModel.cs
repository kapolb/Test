using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kontakt.BL;
using Kontakt.DataModel;

namespace KontaktStatisticsWeb.Models
{
    public class StatInfoModel
    {
        public StatInfoModel() : this(new StatisticInfo())
        {
        }

        public StatInfoModel(StatisticInfo info)
        {
            if (info == null)
                info = new StatisticInfo();
            this.FiltersForData = new List<IdValue>();
            this.FiltersForOrdered = new List<IdValue>();
            this.FiltersForOnStock = new List<IdValue>();
            this.AllUsages = new List<string>();
            this.AllTemplates = new List<string>();
            this.StatInfo = info;
        }

        public StatisticInfo StatInfo { get; private set; }
        public IList<IdValue> FiltersForData { get; set; }
        public IList<IdValue> FiltersForOrdered { get; set; }
        public IList<IdValue> FiltersForOnStock { get; set; }
        public IList<string> AllUsages { get; set; }
        public IList<string> AllTemplates { get; set; }
    }
}