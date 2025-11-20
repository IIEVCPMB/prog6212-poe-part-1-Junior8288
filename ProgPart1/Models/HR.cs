using Microsoft.AspNetCore.Identity;

namespace ProgPart1.Models
{
    
        public class HR : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public decimal HourlyRate { get; set; }
        }

    
}
