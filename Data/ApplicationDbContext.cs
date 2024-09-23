using API.Models.Accounts;
using API.Models.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new() {
                new IdentityRole {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole {
                    Name = "User",
                    NormalizedName = "USER",
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            // Seed default admin user
            //var adminId = Guid.NewGuid().ToString(); // No need because AppUser generate GUID by default
            var hasher = new PasswordHasher<AppUser>();

            AppUser rootAdminUser = new AppUser
            {
                UserName = "quocdatadmin",
                NormalizedUserName = "QUOCDATADMIN",
                Email = "datvipcrvn@gmail.com",
                NormalizedEmail = "DATVIPCRVN@GMAIL.COM",
                EmailConfirmed = true,
            };

            rootAdminUser.PasswordHash = hasher.HashPassword(rootAdminUser, "shinichi");

            builder.Entity<AppUser>().HasData(rootAdminUser);

            // Assign admin role to the user
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roles.First(r => r.Name == "Admin").Id,
                UserId = rootAdminUser.Id,
            });
        }
    }
}