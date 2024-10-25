using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class AppUser : EntityLog 
    {
        public int AppUserId { get; set; }
        public int CompanyEmployeeId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int AppUserTypeId { get; set; }
    }
}
