using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kontakt.BL
{
    public class UsagePrevious
    {
        public UsagePrevious()
        {
        }
        public UsagePrevious(string code, double usage)
        {
            this.Code = code;
            this.Usage = usage;
        }
        public string Code { get; set; }
        public double Usage { get; set; }
    }
}
