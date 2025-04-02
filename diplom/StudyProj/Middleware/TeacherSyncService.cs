using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace StudyProj.Middleware
{
    public class TeacherSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TeacherSyncService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var identityContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                    var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                    await SyncTeachers(identityContext, appContext);
                }

                // Ждём 1 час перед следующим выполнением
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task SyncTeachers(UsersDbContext identityContext, ApplicationContext appContext)
        {
            var teacherRoleId = await identityContext.Roles
                .Where(r => r.Name == "Teacher")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (teacherRoleId == null)
            {
                Console.WriteLine("Роль 'Учитель' не найдена!");
                return;
            }

            // Получаем пользователей с ролью "Учитель"
            var teachers = await (from user in identityContext.Users
                                  join userRole in identityContext.UserRoles
                                  on user.Id equals userRole.UserId
                                  where userRole.RoleId == teacherRoleId
                                  select new
                                  {
                                      user.LastName,
                                      user.FirstName,
                                      user.FatherName
                                  }).ToListAsync();

            // Проверяем, есть ли учитель уже во второй базе
            foreach (var teacher in teachers)
            {
                bool exists = await appContext.Teachers
                    .AnyAsync(t => t.Name == $"{teacher.LastName} {teacher.FirstName} {teacher.FatherName}");

                if (!exists)
                {
                    appContext.Teachers.Add(new Teacher
                    {
                        Name = $"{teacher.LastName} {teacher.FirstName} {teacher.FatherName}",
                    });
                }
            }

            await appContext.SaveChangesAsync();
        }
    }
}
