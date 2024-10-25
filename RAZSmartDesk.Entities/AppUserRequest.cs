using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class AppUserRequest
    {

        public int AppUserRequestId { get; set; }
        public int RequesterId { get; set; }
        public int ApproverId { get; set; } = 0;
        public int RequestTypeId { get; set; }
        public string? RequestDescription { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int RequestStatusId { get; set; }
    }
}
