using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class AppUserTransaction
    {
        public int AppUserTransactionId { get; set; }
        public int AppUserId { get; set; }
        public int CompanyId { get; set; }
        public string? TransactionName { get; set; }
        public int TransactionTypeId { get; set; }
        public string? TransactionDetails { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? StatusRemarks { get; set; }
    }
}
