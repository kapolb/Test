using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KontaktStatisticsWeb.Models
{
    public class StatisticDefinition
    {
        public static IList<Kontakt.BL.StatisticInfo> All { get; set; }
        public Kontakt.BL.StatisticInfo Selected { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Je nutne vybrat hodnotu.")]
        public int Id { get; set; }
        public string GeneratedFilePath { get; set; }
        public string FileName { get; set; }
    }
}