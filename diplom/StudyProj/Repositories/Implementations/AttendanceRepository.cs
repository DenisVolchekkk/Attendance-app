using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudyProj.Repositories.Interfaces;
namespace StudyProj.Repositories.Implementations
{
    public class AttendanceService : BaseRepository<Attendance>, IAttendanceService
    {
        public AttendanceService(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Attendance>> GetAllAsync(Attendance attendance, string currentUserName = null, bool isChief = false , bool isTeacher = false, bool isAdmin = false)
        {
            var Attendances = Context.Set<Attendance>().AsQueryable();

            // Фильтрация для Teacher и Chief
            if (!string.IsNullOrEmpty(currentUserName) && !isAdmin)
            {
                if (isTeacher)
                {
                    Attendances = Attendances.Where(a => a.Schedule.Teacher.Name == currentUserName);
                }
                else if (isChief)
                {
                    var chief = Context.Chiefs.FirstOrDefault(c => c.Name == currentUserName);
                    if (chief != null && chief.GroupId != null)
                    {
                        // Получаем посещения, связанные с группой этого старосты
                        Attendances = Attendances.Where(a => a.Schedule.GroupId == chief.GroupId);
                    }
                    else
                    {
                        // Если пользователь не староста или группа не назначена, возвращаем пустой список
                        Attendances = Enumerable.Empty<Attendance>().AsQueryable();
                    }
                }
            }
            if (attendance.AttendanceDate != null)
            {
                Attendances = Attendances.Where(d => d.AttendanceDate == attendance.AttendanceDate);
            }
            if (attendance.Schedule != null && attendance.Schedule.StartTime.TotalMinutes != 0)
            {
                Attendances = Attendances.Where(d => d.Schedule.StartTime == attendance.Schedule.StartTime);
            }
            if (attendance.Student != null && !string.IsNullOrEmpty(attendance.Student.Name))
            {
                Attendances = Attendances.Where(d => d.Student.Name.Contains(attendance.Student.Name));
            }
            if (attendance.Schedule != null && attendance.Schedule.Discipline != null && !string.IsNullOrEmpty(attendance.Schedule.Discipline.Name))
            {
                Attendances = Attendances.Where(d => d.Schedule.Discipline.Name == attendance.Schedule.Discipline.Name);
            }
            //if (attendance.Schedule != null && attendance.Schedule.Group != null && attendance.Schedule.Group.Chief != null && !string.IsNullOrEmpty(attendance.Schedule.Group.Chief.Name))
            //{
            //    Attendances = Attendances.Where(d => d.Schedule.Group.Chief.Name == attendance.Schedule.Group.Chief.Name);
            //}
            if (attendance.Schedule != null && attendance.Schedule.Teacher != null && !string.IsNullOrEmpty(attendance.Schedule.Teacher.Name))
            {
                Attendances = Attendances.Where(d => d.Schedule.Teacher.Name == attendance.Schedule.Teacher.Name);
            }
            if (attendance.Schedule != null && attendance.Schedule.Group != null && !string.IsNullOrEmpty(attendance.Schedule.Group.Name))
            {
                Attendances = Attendances.Where(d => d.Schedule.Group.Name == attendance.Schedule.Group.Name);
            }
            //if (attendance.Schedule != null && attendance.Schedule.Group != null && attendance.Schedule.Group.Chief != null && !string.IsNullOrEmpty(attendance.Schedule.Group.Chief.Name))
            //{
            //    Attendances = Attendances.Where(d => d.Schedule.Group.Chief.Name == attendance.Schedule.Group.Chief.Name);
            //}
            if (attendance.Schedule != null && attendance.Schedule.DayOfWeek != null)
            {
                Attendances = Attendances.Where(d => d.Schedule.DayOfWeek == attendance.Schedule.DayOfWeek);
            }
            Attendances = Attendances.OrderByDescending(a => a.AttendanceDate)
                       .ThenByDescending(a => a.Schedule.StartTime);
            return await Attendances.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
