using Microsoft.AspNetCore.Identity;

namespace Ass1_C_5_OrderFastFood.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }        // 1
        public string? Address { get; set; }         // 2
        public DateTime? DateOfBirth { get; set; }  // 3
        public string? Gender { get; set; }          // 4
        public string? AvatarUrl { get; set; }       // 5 (optional)
        public string? AdditionalInfo { get; set; }
    }
}
