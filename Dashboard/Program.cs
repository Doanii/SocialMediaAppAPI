using Dashboard.Data;
using Dashboard.Hubs;
using Dashboard.MiddlewareExtentions;
using Dashboard.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<DashboardDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

            builder.Services.AddSingleton<DashboardHub>();
            builder.Services.AddSingleton<PostTableDependency>();
            builder.Services.AddSingleton<UserTableDependency>();
            builder.Services.AddSingleton<ActivityTableDependency>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<DashboardHub>("/DashboardHub");
            app.UsePhotoTableDependency(connectionString);

            app.Run();
        }
    }
}
