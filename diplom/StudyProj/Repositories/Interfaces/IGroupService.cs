using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IGroupService : IBaseRepository<Group>
{
    Task<List<Group>> GetAllAsync(Group group);
}