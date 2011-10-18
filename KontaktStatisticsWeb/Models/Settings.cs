using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontaktStatisticsWeb.Models
{
    public class Settings
    {
        Kontakt.BL.StatisticInfo Statistic { get; set; }
        public IList<Kontakt.BL.StatisticInfo> All { get; set; }
    }
}