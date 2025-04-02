using Domain.Models;
using Microsoft.EntityFrameworkCore;
using StudyProj.Repositories.Interfaces;
namespace StudyProj.Repositories.Implementations
{
    public class AttendanceService : BaseRepository<Attendance>, IAttendanceService
    {
        public AttendanceService(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Attendance>> GetAllAsync(Attendance attendance)
        {
            var Attendances = Context.Set<Attendance>().AsQueryable();
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


            return await Attendances.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
