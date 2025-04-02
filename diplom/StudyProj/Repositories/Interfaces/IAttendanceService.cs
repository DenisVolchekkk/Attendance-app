using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IAttendanceService : IBaseRepository<Attendance>
{
    Task<List<Attendance>> GetAllAsync(Attendance attendance);
}