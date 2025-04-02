using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StudyProj.Middleware
{
    public class AttendanceGeneratorService
    {
        private readonly ApplicationContext _context;

        public AttendanceGeneratorService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task GenerateAttendanceRecordsAsync(DateTime targetDate)
        {
            // Получить все расписания на указанный день недели
            var schedules = await _context.Schedules
                .Include(s => s.Group)
                .ToListAsync();

            // Создаем список всех дат, включая targetDate и 10 предыдущих дней
            var datesToGenerate = Enumerable.Range(0, 11)
                .Select(offset => targetDate.AddDays(-offset))
                .ToList();

            foreach (var schedule in schedules)
            {
                // Для каждой даты проверяем, соответствует ли она нужному дню недели в расписании
                foreach (var date in datesToGenerate)
                {
                    // Если день недели текущей даты совпадает с днем недели из расписания
                    if (date.DayOfWeek == schedule.DayOfWeek)
                    {
                        // Получить студентов группы
                        var students = await _context.Students
                            .Where(s => s.GroupId == schedule.GroupId)
                            .ToListAsync();

                        foreach (var student in students)
                        {
                            // Проверить, существует ли уже запись на эту дату для данного студента и расписания
                            bool exists = await _context.Attendance.AnyAsync(a =>
                                a.StudentId == student.Id &&
                                a.ScheduleId == schedule.Id &&
                                a.AttendanceDate == date);

                            if (!exists)
                            {
                                // Создать запись посещения
                                var attendance = new Attendance
                                {
                                    StudentId = student.Id,
                                    ScheduleId = schedule.Id,
                                    AttendanceDate = date,
                                    IsPresent = false // По умолчанию "не присутствовал"
                                };

                                _context.Attendance.Add(attendance);
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }


}
