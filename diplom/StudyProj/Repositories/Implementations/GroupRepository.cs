using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StudyProj.Repositories.Implementations
{
    public class GroupService : BaseRepository<Group>, IGroupService
    {
        public GroupService(ApplicationContext context) : base(context)
        {
        }
        public async Task<List<Group>> GetAllAsync(Group group)
        {
            var Groups = Context.Set<Group>().AsQueryable();
            if (!string.IsNullOrEmpty(group.Name))
            {
                Groups = Groups.Where(d => d.Name == group.Name);
            }
            //if(group.Chief != null && !string.IsNullOrEmpty(group.Chief.Name))
            //{
            //    Groups = Groups.Where(d => d.Chief.Name == group.Chief.Name);
            //}
            if (group.Facility != null && !string.IsNullOrEmpty(group.Facility.Name))
            {
                Groups = Groups.Where(d => d.Facility.Name == group.Facility.Name);
            }
            return await Groups.ToListAsync();  // Асинхронное получение всех записей
        }
    }
}
