using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsApplication.DAL.Models;

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
            builder.Entity<News>().HasQueryFilter(a => a.IsDeleted == false);
            builder.Entity<ApplicationUser>().HasQueryFilter(a => a.IsDeleted == false);

            var identityRoles = new List<IdentityRole>()
            {
                new IdentityRole(){ Name= "Normal",NormalizedName="NORMAL"} ,
                new IdentityRole(){ Name= "ContentAdmin",NormalizedName="CONTENTADMIN "} ,
                new IdentityRole() {Name= "Admin",NormalizedName="ADMIN" } ,
            };

            builder.Entity<IdentityRole>().HasData(identityRoles);

            base.OnModelCreating(builder);  
        }
    }
}
