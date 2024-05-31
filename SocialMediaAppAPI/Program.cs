
using Dashboard.Data;
using Dashboard.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Types.Middlewares;

namespace SocialMediaAppAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.SocialMediaAppAPI.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<APIDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("ApiToken", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "ApiToken",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "ApiToken header for API requests",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiToken"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            app.UseMiddleware<ApiTokenMiddleware>();

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
