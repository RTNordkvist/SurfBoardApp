using Microsoft.AspNetCore.Identity;

namespace SurfBoardApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? FullName
        {
            get
            {
                if (FirstName != null && LastName != null) {return FirstName + " " + LastName; }
                else { return null; }
            }
        }
    }
}
