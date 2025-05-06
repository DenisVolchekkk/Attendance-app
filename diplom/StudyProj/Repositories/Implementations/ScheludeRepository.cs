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
                    var chief = Context.Chiefs.FirstOrDefault(c => c.Name == currentUserName);
                    if (chief != null && chief.GroupId != null)
                    {
                        // Получаем посещения, связанные с группой этого старосты
                        schedules = schedules.Where(a => a.GroupId == chief.GroupId);
                    }
                    else
                    {
                        // Если пользователь не староста или группа не назначена, возвращаем пустой список
                        schedules = Enumerable.Empty<Schedule>().AsQueryable();
                    }
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
                schedules = schedules.Where(d => d.Teacher.Name == schedule.Teacher.Name);
            }
            if (schedule.Discipline != null && !string.IsNullOrEmpty(schedule.Discipline.Name))
            {
                schedules = schedules.Where(d => d.Discipline.Name == schedule.Discipline.Name);
            }
            if (schedule.Semestr != null)
            {
                schedules = schedules.Where(d => d.Semestr == schedule.Semestr);
            }
            if (schedule.StudyYear != null )
            {
                schedules = schedules.Where(d => d.StudyYear == schedule.StudyYear);
            }
            schedules = schedules.OrderBy(s => s.DayOfWeek)
                     .ThenBy(s => s.StartTime);
            return await schedules.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
