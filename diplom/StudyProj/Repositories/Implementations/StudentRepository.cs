using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StudyProj.Repositories.Implementations
{
    public class StudentService : BaseRepository<Student>, IStudentService
    {
        public StudentService(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Student>> GetAllAsync(Student student)
        {
            var students = Context.Set<Student>().AsQueryable();
            if (!string.IsNullOrEmpty(student.Name))
            {
                students = students.Where(d => d.Name.Contains(student.Name));
            }
            if(student.Group != null && !string.IsNullOrEmpty(student.Group.Name))
            {
                students = students.Where(d => d.Group.Name.Contains(student.Group.Name));
            }

            return await students.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
