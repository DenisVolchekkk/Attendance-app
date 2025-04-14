using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IScheduleService : IBaseRepository<Schedule>
{
    Task<List<Schedule>> GetAllAsync(Schedule schedule, string currentUserName = null, bool isChief = false, bool isTeacher = false, bool isAdmin = false);
}