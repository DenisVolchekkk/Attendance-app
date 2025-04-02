using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IStudentService : IBaseRepository<Student>
{
    Task<List<Student>> GetAllAsync(Student student);
}