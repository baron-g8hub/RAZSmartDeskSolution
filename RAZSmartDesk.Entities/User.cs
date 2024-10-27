using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAZSmartDesk.Entities
{
    public class User : EntityLog 
    {
        public int UserId { get; set; }
        public int UserCompanyId { get; set; }
        public int UserTypeId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? UserTypeName { get; set; }
        public int UserTypeLevel { get; set; } = 0;

    }
}
