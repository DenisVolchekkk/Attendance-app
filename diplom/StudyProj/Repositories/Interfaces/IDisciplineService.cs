using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IDisciplineService : IBaseRepository<Discipline>
{
    Task<List<Discipline>> GetAllAsync(Discipline discipline);
}
