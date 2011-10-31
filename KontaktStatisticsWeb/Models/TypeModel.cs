using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontaktStatisticsWeb.Models
{
    public class TypeModel
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public static IList<TypeModel> GetAllTypes()
        {
            List<TypeModel> types = new List<TypeModel>();
            types.Add(new TypeModel() { Name = "Káble filtrované", Type = "Kontakt.BL.CablesDataFiltered" });
            types.Add(new TypeModel() { Name = "Všeobecné", Type = "Kontakt.BL.CablesData" });
            return types;
        }
    }
}