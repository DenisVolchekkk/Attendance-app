using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IScheduleService : IBaseRepository<Schedule>
{
    Task<List<Schedule>> GetAllAsync(Schedule schedule);
}