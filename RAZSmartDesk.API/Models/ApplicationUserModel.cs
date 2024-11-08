using RAZSmartDesk.Entities;

namespace RAZSmartDesk.API.Models
{
    public class ApplicationUserModel
    {
        public int ApplicationUserId { get; set; } = 0;
        public string ApplicationUserToken { get; set; } = string.Empty;
        public string ApplicationUsername { get; set; } = string.Empty;
        public string ApplicationUserPassword { get; set; } = string.Empty;
        public int ApplicationUserTypeId { get; set; } = 0;
        public string ApplicationUserTypeName { get; set; } = string.Empty; 
    }
}
