using Domain.Models;
namespace StudyProj.Repositories.Interfaces
{
    public interface IBaseRepository<TDbModel> where TDbModel : BaseModel
    {
        Task<List<TDbModel>> GetAllAsync();
        Task<List<TDbModel>> GetAllAsync(TDbModel model);
        Task<TDbModel> GetAsync(int id);
        Task<TDbModel> CreateAsync(TDbModel model);
        Task<TDbModel> UpdateAsync(TDbModel model);
        Task DeleteAsync(int id);
    }
}
