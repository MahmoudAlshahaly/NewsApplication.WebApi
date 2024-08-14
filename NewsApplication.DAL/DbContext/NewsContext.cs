using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsApplication.DAL.Models;
using System.Reflection.Emit;

namespace NewsApplication.DAL.DbContext
{
    public class NewsContext:IdentityDbContext<ApplicationUser>
    {
        public NewsContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<News> News  { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<News>().HasQueryFilter(a => !a.IsDeleted);
            builder.Entity<ApplicationUser>().HasQueryFilter(a => !a.IsDeleted);

            // Seed roles
            string normalRoleId = "NR1";
            string contentAdminRoleId = "CAR2";
            string adminRoleId = "AR3";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = normalRoleId , Name = "Normal", NormalizedName = "NORMAL" },
                new IdentityRole() { Id = contentAdminRoleId, Name = "ContentAdmin", NormalizedName = "CONTENTADMIN " },
                new IdentityRole() { Id = adminRoleId , Name = "Admin", NormalizedName = "ADMIN" }
            );

            // Seed users
            string normalUserId = "NU1";
            string contentAdminUserId = "CAU2";
            string adminUserId = "AU3";

            var hasher = new PasswordHasher<ApplicationUser>();

            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminUserId,
                    UserName = "mahmoud",
                    NormalizedUserName = "MAHMOUD",
                    Email = "mahmoud@domain.com",
                    NormalizedEmail = "MAHMOUD@DOMAIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "MAHMOUD@123"),
                    SecurityStamp = string.Empty,
                    DateOfBirth= DateTime.UtcNow.AddYears(-24)
                },
                new ApplicationUser
                {
                    Id = contentAdminUserId,
                    UserName = "ahmed",
                    NormalizedUserName = "AHMED",
                    Email = "ahmed@domain.com",
                    NormalizedEmail = "AHMED@DOMAIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "AHMED@123"),
                    SecurityStamp = string.Empty,
                    DateOfBirth = DateTime.UtcNow.AddYears(-25)
                },
                new ApplicationUser
                {
                    Id = normalUserId,
                    UserName = "ayman",
                    NormalizedUserName = "AYMAN",
                    Email = "ayman@domain.com",
                    NormalizedEmail = "AYMAN@DOMAIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "AYMAN@123"),
                    SecurityStamp = string.Empty,
                    DateOfBirth = DateTime.UtcNow.AddYears(-27)
                }
            );

            // Seed user roles
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = normalUserId, RoleId = normalRoleId },
                new IdentityUserRole<string> { UserId = contentAdminUserId, RoleId = contentAdminRoleId }
            );


            // Seed News Data
            builder.Entity<News>().HasData(
                new News
                {
                    Id = 1,
                    Title = "Breaking News",
                    TitleAr = "أخبار عاجلة",
                    Description = "This is a breaking news story.",
                    DescriptionAr = "هذه قصة إخبارية عاجلة.",
                    PublishDate = DateTime.Now.AddDays(-10),
                    ImageURL = "/images/breaking-news.jpg",
                    IsDeleted = false,
                    ApplicationUserId = normalUserId
                },
                new News
                {
                    Id = 2,
                    Title = "Technology News",
                    TitleAr = "أخبار التكنولوجيا",
                    Description = "Latest updates in technology.",
                    DescriptionAr = "أحدث التحديثات في التكنولوجيا.",
                    PublishDate = DateTime.Now.AddDays(2),
                    ImageURL = "/images/technology-news.jpg",
                    IsDeleted = false,
                    ApplicationUserId = contentAdminUserId
                },
                new News
                {
                    Id = 3,
                    Title = "Sports News",
                    TitleAr = "أخبار الرياضة",
                    Description = "Latest sports updates.",
                    DescriptionAr = "آخر التحديثات الرياضية.",
                    PublishDate = DateTime.Now.AddDays(-3),
                    ImageURL = "/images/sports-news.jpg",
                    IsDeleted = false,
                    ApplicationUserId = adminUserId
                }
            );


            base.OnModelCreating(builder);  
        }
    }
}
