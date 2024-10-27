using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class UserTransaction
    {
        public int UserTransactionId { get; set; } = 0;
        public int? UserId { get; set; }
        public int CompanyId { get; set; }
        public string? TransactionName { get; set; }
        public int TransactionTypeId { get; set; }
        public string? TransactionDetails { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? StatusRemarks { get; set; }
    }
}
