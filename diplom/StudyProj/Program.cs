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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin() // Разрешить запросы с любого источника
                          .AllowAnyMethod() // Разрешить все HTTP-методы (GET, POST, PUT, DELETE и т.д.)
                          .AllowAnyHeader(); // Разрешить все заголовки
                });
            });
            //builder.Services.AddHostedService<AttendanceGenerationService>();
            builder.Services.AddAutoMapper(typeof(Program));
            // Add builder.Services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
            },
            Array.Empty<string>()
        }
    });

            });
            string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            string userDbConnection = builder.Configuration.GetConnectionString("UserDbConnection");

            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseLazyLoadingProxies().UseMySQL(defaultConnection));

            builder.Services.AddDbContext<UsersDbContext>(options =>
                options.UseMySQL(userDbConnection));

            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2)); 

            var jwtSettings = builder.Configuration.GetSection("JWTSettings");
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddTransient<AttendanceGeneratorService>();
            builder.Services.AddTransient<IDisciplineService, DisciplineService>();
            builder.Services.AddTransient<IChiefService, ChiefService>();
            builder.Services.AddTransient<IFacilityService, FacilityService>();
            builder.Services.AddTransient<IGroupService, GroupService>();
            builder.Services.AddTransient<IAttendanceService, AttendanceService>();
            builder.Services.AddTransient<IScheduleService, ScheludeService>();
            builder.Services.AddTransient<IStudentService, StudentService>();
            builder.Services.AddTransient<ITeacherService,TeacherService>();

            builder.Services.AddHostedService<AttendanceBackgroundService>();
            builder.Services.AddHostedService<TeacherSyncService>();

            builder.Services.AddSingleton<JwtHandler>();

            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);

            builder.Services.AddScoped<IEmailSender, EmailSender>();

            builder.Services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;

            });

            builder.Services.AddMvc();
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
