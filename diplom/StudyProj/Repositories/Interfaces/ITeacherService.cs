using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface ITeacherService : IBaseRepository<Teacher>
{
    Task<List<Teacher>> GetAllAsync(Teacher teacher);
}