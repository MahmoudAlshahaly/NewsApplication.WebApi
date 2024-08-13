using Microsoft.Extensions.DependencyInjection;
using NewsApplication.BLL.AutoMapper;
using NewsApplication.BLL.Managers.AccountManager;
using NewsApplication.BLL.Services;
using Shipping.BLL.Managers;

namespace NewsApplication.BLL
{
    public static class DependencyInjectionBLL
    {
        public static void AddBusinessLogicLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(map => map.AddProfile(new DomainProfile()));
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<INewsManager, NewsManager>();
            services.AddHostedService<NewsCleanUpService>();
        }
    }
}
