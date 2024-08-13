
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsApplication.BLL;
using NewsApplication.DAL;
using NewsApplication.DAL.DbContext;
using NewsApplication.DAL.Models;
using System.Text;

namespace NewsApplication.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddBusinessLogicLayer();
            builder.Services.AddDataAccessLayer(builder.Configuration);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                //options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<NewsContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllers();


            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JWT";
                options.DefaultChallengeScheme = "JWT";

            }).AddJwtBearer("JWT", options =>
            {
                var secretKey = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("SecretKey").Value ?? string.Empty);
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
            });

            // Adding Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("AdminAndContentAdmin", policy =>
                {
                    policy.RequireRole("Admin", "ContentAdmin");
                });
                options.AddPolicy("NormalAndAdminAndContentAdmin", policy =>
                {
                    policy.RequireRole("Normal","Admin", "ContentAdmin");
                });
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // Ensure no filters are excluding any methods
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}