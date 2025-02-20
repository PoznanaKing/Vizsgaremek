
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {

                options.AddPolicy(MyAllowSpecificOrigins,
                                      policy =>
                                      {
                                          policy.WithOrigins("http://localhost:3000"
                                                             )
                                                                .AllowAnyHeader()
                                                                .AllowAnyMethod();
                                      });
            });



            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddScoped<IPost, PostService>();
            builder.Services.AddScoped<IEmail, EmailService>();
            builder.Services.AddScoped<IAuth, AuthService>();
            builder.Services.AddScoped<IPostComment, PostCommentService>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenarator>();
            builder.Services.AddScoped<IPlace, PlaceService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("AuthSettings:JwtOptions"));


            var settingsSection = builder.Configuration.GetSection("AuthSettings:JwtOptions");

            var secret = settingsSection.GetValue<string>("Secret");
            var issuer = settingsSection.GetValue<string>("Issuer");
            var auidience = settingsSection.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = auidience,
                    ValidateAudience = true
                };
            });

            builder.Services.AddAuthorization();




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
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();


            
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Swagger konfiguráció
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Image Upload API", Version = "v1" });

                // Swagger típus hozzáadása az IFormFile-hez
                c.MapType<IFormFile>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image Upload API v1");
                c.RoutePrefix = string.Empty; // Ha a Swagger UI-t a root szinten akarod
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
