using Microsoft.AspNetCore.Mvc.Rendering;
using RAZSmartDesk.Entities;
using System.ComponentModel.DataAnnotations;

namespace RAZSmartDesk.WebUI.Models
{
    public class UsersViewModel
    {
        public UsersViewModel()
        {
            Entity = new User();
            EntityList = new List<User>();
            SelectListUserTypes = LoadUserTypes();
        }

        public User Entity { get; set; }
        public List<User> EntityList { get; set; }
        public List<SelectListItem> SelectListUserTypes { get; set; }

        public List<SelectListItem> LoadUserTypes()
        {
            var types = new List<SelectListItem>()
            {
                new SelectListItem { Value = "1", Text = "ADMIN" },
                new SelectListItem { Value = "2", Text = "FLAT" },
            };
            return types;
        }


        public int ApplicationUserId { get; set; }

        public int CompanyId { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
