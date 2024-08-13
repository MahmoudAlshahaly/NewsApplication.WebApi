using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsApplication.DAL.DbContext;
using NewsApplication.DAL.Models;
using NewsApplication.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsApplication.DAL
{
    public static class DependencyInjectionDAL
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NewsContext>(options => options.UseSqlServer(configuration.GetConnectionString("conn")));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
           
        }
    }
}
