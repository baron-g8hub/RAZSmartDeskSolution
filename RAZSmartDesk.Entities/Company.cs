using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class Company : EntityLog
    {

        public int CompanyId { get; set; } = 0;
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyDescription { get; set; }
        public string? ExtraString1 { get; set; }
        public string? ExtraString2 { get; set; }
        public int ExtraInt1 { get; set; }
        public int ExtraInt2 { get; set; }


    }
}
