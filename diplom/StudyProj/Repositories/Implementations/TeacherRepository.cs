using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StudyProj.Repositories.Implementations
{
    public class TeacherService : BaseRepository<Teacher>, ITeacherService
    {
        public TeacherService(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Teacher>> GetAllAsync(Teacher teacher)
        {
            var teachers = Context.Set<Teacher>().AsQueryable();
            if (!string.IsNullOrEmpty(teacher.Name))
            {
                teachers = teachers.Where(d => d.Name.Contains(teacher.Name));
            }
            //if (teacher.Facility != null && !string.IsNullOrEmpty(teacher.Facility.Name))
            //{
            //    teachers = teachers.Where(d => d.Facility.Name.Contains(teacher.Facility.Name));
            //}

            return await teachers.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
