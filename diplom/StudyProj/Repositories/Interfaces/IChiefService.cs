using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IChiefService : IBaseRepository<Chief>
{
    Task<List<Chief>> GetAllAsync(Chief chief);
}