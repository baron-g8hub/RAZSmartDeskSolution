using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class CompanyAccounts : EntityLog
    {
        public int CompanyAccountId { get; set; }
        public int CompanyId { get; set; }
        public int AccountCodeId { get; set; }
        public string? CompanyAccountDescription { get; set; }
    }
}
