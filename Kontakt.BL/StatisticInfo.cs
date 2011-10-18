using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kontakt.BL
{
    public class StatisticInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string TemplateName { get; set; }
        public string GenerateName { get; set; }
        public int FilterForDataId { get; set; }
        public string FilterForData { get; set; }
        public int FilterForOrderedId { get; set; }
        public string FilterForOrdered { get; set; }
        public int FilterForOnStockId { get; set; }
        public string FilterForOnStock { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
