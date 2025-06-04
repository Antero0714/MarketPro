using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MarketPro.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MarketPro.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)        
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles with proper configuration
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole 
                { 
                    Id = "1", 
                    Name = "Admin", 
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "a1b2c3d4-e5f6-4a5b-8c7d-9e0f1a2b3c4d"
                },
                new IdentityRole 
                { 
                    Id = "2", 
                    Name = "User", 
                    NormalizedName = "USER",
                    ConcurrencyStamp = "b2c3d4e5-f6a7-5b6c-9d0e-1f2a3b4c5d6e"
                }
            );

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
