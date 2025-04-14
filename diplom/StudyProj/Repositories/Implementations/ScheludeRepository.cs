using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StudyProj.Repositories.Implementations
{
    public class ScheludeService : BaseRepository<Schedule>, IScheduleService
    {
        public ScheludeService(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Schedule>> GetAllAsync(Schedule schedule, string currentUserName = null, bool isChief = false, bool isTeacher = false, bool isAdmin = false)
        {
            var schedules = Context.Set<Schedule>().AsQueryable();
            if (!string.IsNullOrEmpty(currentUserName) && !isAdmin)
            {
                if (isTeacher)
                {
                    schedules = schedules.Where(a => a.Teacher.Name == currentUserName);
                }
                else if (isChief)
                {
                    schedules = schedules.Where(a => a.Group.Chief.Name == currentUserName);
                }
            }
            if (schedule.DayOfWeek != null)
            {
                schedules = schedules.Where(d => d.DayOfWeek == schedule.DayOfWeek);
            }
            if (schedule.StartTime.TotalMinutes != 0)
            {
                schedules = schedules.Where(d => d.StartTime == schedule.StartTime);
            }
            if (schedule.Teacher != null && !string.IsNullOrEmpty(schedule.Teacher.Name))
            {
                schedules = schedules.Where(d => d.Teacher.Name.Contains(schedule.Teacher.Name));
            }
            if (schedule.Discipline != null && !string.IsNullOrEmpty(schedule.Discipline.Name))
            {
                schedules = schedules.Where(d => d.Discipline.Name.Contains(schedule.Discipline.Name));
            }
            schedules = schedules.OrderBy(s => s.DayOfWeek)
                     .ThenBy(s => s.StartTime);
            return await schedules.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
