
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace AuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddScoped<IPost, PostService>();
            builder.Services.AddScoped<IEmail, EmailService>();
            builder.Services.AddScoped<IAuth, AuthService>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenarator>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("AuthSettings:JwtOptions"));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
