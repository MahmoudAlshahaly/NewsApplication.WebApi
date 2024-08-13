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

            base.OnModelCreating(builder);  
        }
    }
}
