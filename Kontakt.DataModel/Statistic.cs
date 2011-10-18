using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kontakt.DataModel
{
    public class Statistic
    {
        public Statistic()
        {
            this.PreviousUsages = new List<Usage>();
        }

        public string Name { get; set; }
        public int SKzId { get; set; }
        public List<Usage> PreviousUsages { get; set; }
        public decimal Weight { get; set; }
        public UnitType UnitType { get; set; }
        public decimal WarehouseStockKilograms { get; set; }
        public decimal WarehouseStockMeters { get; set; }
        public decimal WarehouseStockUnits { get; set; }
        public decimal OrderedKilograms { get; set; }
        public decimal OrderedMeters { get; set; }
        public decimal OrderedUnits { get; set; }
        public decimal OrderKilograms { get; set; }
        public decimal OrderMeters { get; set; }
        public decimal OrderUnits { get; set; }
    }

    public enum UnitType
    {
        Meter,
        Kilogram
    }

    public class Usage
    {
        public double UsageKilograms { get; set; }
        public double UsageMeters { get; set; }
        public double UsageUnits { get; set; }
        public double OffcutKilograms { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
