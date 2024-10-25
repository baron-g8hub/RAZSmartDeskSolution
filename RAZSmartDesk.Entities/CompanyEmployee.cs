using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class CompanyEmployee : EntityLog
    {
        public int CompanyEmployeeId { get; set; }
        public int CompanyId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public DateTime HiredDate { get; set; }
        public string? CompanyPosition { get; set; }
    }
}
