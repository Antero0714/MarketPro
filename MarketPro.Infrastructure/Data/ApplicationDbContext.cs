using MarketPro.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarketPro.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var admin = new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" };
            var client = new IdentityRole { Id = "2", Name = "client", NormalizedName = "CLIENT" };
            var seller = new IdentityRole { Id = "3", Name = "seller", NormalizedName = "SELLER" };

            builder.Entity<IdentityRole>().HasData(admin, client,seller);

        }
    }
}
