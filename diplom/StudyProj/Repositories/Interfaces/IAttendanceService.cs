using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IAttendanceService : IBaseRepository<Attendance>
{
    Task<List<Attendance>> GetAllAsync(Attendance attendance, string currentUserName, bool isChief, bool isTeacher, bool isAdmin);
}