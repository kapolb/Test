using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Kontakt.BL
{
    public class StatisticInfo
    {
        public StatisticInfo()
        {
            this.Active = true;
        }

        public bool Active { get; set; }
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Názov je povinný.")]
        [Display(Name="Názov")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Typ je povinný.")]
        [Display(Name = "Typ generovania")]
        public string ClassName { get; set; }
        
        [Required(ErrorMessage = "Názov pre template je povinný.")]
        [Display(Name = "Názov template")]
        public string TemplateName { get; set; }
        
        [Display(Name = "Názov generovaného súboru")]
        public string GenerateName { get; set; }

        [Display(Name = "Spotreba 2010")]
        public string Usages2010File { get; set; }
        
        [Required(ErrorMessage = "Filter pre dáta je povinný.")]
        [Display(Name = "Filter pre dáta")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Musí byť vybratý filter pre dáta.")]
        public int FilterForDataId { get; set; }
        
        public string FilterForData { get; set; }
        
        [Required(ErrorMessage = "Filter pre objednané je povinný.")]
        [Display(Name = "Filter pre objednané")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Musí byť vybratý filter pre objednané.")]
        public int FilterForOrderedId { get; set; }
        
        public string FilterForOrdered { get; set; }
        
        [Required(ErrorMessage = "Filter pre skladové zásoby je povinný.")]
        [Display(Name = "Filter pre sklad")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Musí byť vybratý filter pre sklad.")]
        public int FilterForOnStockId { get; set; }
        
        public string FilterForOnStock { get; set; }

        public void Fill(StatisticInfo old)
        {
            this.ClassName = old.ClassName;
            this.FilterForData = old.FilterForData;
            this.FilterForDataId = old.FilterForDataId;
            this.FilterForOnStock = old.FilterForOnStock;
            this.FilterForOnStockId = old.FilterForOnStockId;
            this.FilterForOrdered = old.FilterForOrdered;
            this.FilterForOrderedId = old.FilterForOrderedId;
            this.GenerateName = old.GenerateName;
            //this.Id = old.Id;
            this.Name = old.Name;
            this.TemplateName = old.TemplateName;
            this.Usages2010File = old.Usages2010File;
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
