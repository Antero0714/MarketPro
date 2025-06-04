using Microsoft.AspNetCore.Identity;

namespace MarketPro.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
