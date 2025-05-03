using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Domain.Models;
using StudyProj.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StudyProj.JwtFeatures;
using EmailService;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using StudyProj.Middleware;
using StudyProj.Middleware.Middleware;
using StudyProj.Repositories.Interfaces;
namespace StudyProj
{
    public class Program
    {
        public static void Main(string[] args)
         {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureServices();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Создание базы данных для ApplicationContext
                var applicationContext = services.GetRequiredService<ApplicationContext>();
                applicationContext.Database.EnsureCreated();

                // Создание базы данных для UsersDbContext
                var usersDbContext = services.GetRequiredService<UsersDbContext>();
                usersDbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
