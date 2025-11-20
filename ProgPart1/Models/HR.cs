using Microsoft.AspNetCore.Identity;

namespace ProgPart1.Models
{
    public class HR
    {
        public class AppUser : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public decimal HourlyRate { get; set; }
        }

    }
}
