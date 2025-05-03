using Domain.Models;
using EmailService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyProj.JwtFeatures;
using StudyProj.Middleware.Middleware;
using StudyProj.Middleware;
using StudyProj.Repositories.Implementations;
using System.Text;

namespace StudyProj
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            ConfigureCors(builder);
            ConfigureAutoMapper(builder);
            ConfigureControllers(builder);
            ConfigureSwagger(builder);
            ConfigureDbContexts(builder);
            ConfigureIdentity(builder);
            ConfigureAuthentication(builder);
            ConfigureAuthorization(builder);
            ConfigureApplicationServices(builder);
            ConfigureBackgroundServices(builder);
            ConfigureEmailService(builder);
            ConfigureFormOptions(builder);
            ConfigureMvc(builder);
        }

        private static void ConfigureCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }

        private static void ConfigureAutoMapper(WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(typeof(Program));
        }

        private static void ConfigureControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
        }

        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
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
        }

        private static void ConfigureDbContexts(WebApplicationBuilder builder)
        {
            string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            string userDbConnection = builder.Configuration.GetConnectionString("UserDbConnection");

            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseLazyLoadingProxies().UseMySQL(defaultConnection));

            builder.Services.AddDbContext<UsersDbContext>(options =>
                options.UseMySQL(userDbConnection));
        }

        private static void ConfigureIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
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
        }

        private static void ConfigureAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
        }

        private static void ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<AttendanceGeneratorService>();
            builder.Services.AddTransient<IDisciplineService, DisciplineService>();
            builder.Services.AddTransient<IChiefService, ChiefService>();
            builder.Services.AddTransient<IFacilityService, FacilityService>();
            builder.Services.AddTransient<IGroupService, GroupService>();
            builder.Services.AddTransient<IAttendanceService, AttendanceService>();
            builder.Services.AddTransient<IScheduleService, ScheludeService>();
            builder.Services.AddTransient<IStudentService, StudentService>();
            builder.Services.AddTransient<ITeacherService, TeacherService>();
            builder.Services.AddSingleton<JwtHandler>();
        }

        private static void ConfigureBackgroundServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHostedService<AttendanceBackgroundService>();
            builder.Services.AddHostedService<TeacherSyncService>();
            builder.Services.AddHostedService<FacilitySyncService>();
        }

        private static void ConfigureEmailService(WebApplicationBuilder builder)
        {
            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSender>();
        }

        private static void ConfigureFormOptions(WebApplicationBuilder builder)
        {
            builder.Services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        private static void ConfigureMvc(WebApplicationBuilder builder)
        {
            builder.Services.AddMvc();
        }
    }
}
