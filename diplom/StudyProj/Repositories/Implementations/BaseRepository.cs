using Microsoft.EntityFrameworkCore;
using Domain.Models;
using StudyProj.Repositories.Interfaces;
namespace StudyProj.Repositories.Implementations
{
    public class BaseRepository<TDbModel> : IBaseRepository<TDbModel> where TDbModel : BaseModel
    {
        public ApplicationContext Context { get; set; }

        public BaseRepository(ApplicationContext context)
        {
            Context = context;
        }

        public async Task<TDbModel> CreateAsync(TDbModel model)
        {
            await Context.Set<TDbModel>().AddAsync(model);  // Асинхронное добавление
            await Context.SaveChangesAsync();  // Асинхронное сохранение
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var toDelete = await Context.Set<TDbModel>().FirstOrDefaultAsync(m => m.Id == id);  // Асинхронный поиск
            if (toDelete != null)
            {
                Context.Set<TDbModel>().Remove(toDelete);
                await Context.SaveChangesAsync();  // Асинхронное сохранение
            }
        }

        public async Task<List<TDbModel>> GetAllAsync()
        {
            return await Context.Set<TDbModel>().ToListAsync();  // Асинхронное получение всех записей
        }

        public async Task<TDbModel> UpdateAsync(TDbModel model)
        {
            var toUpdate = await Context.Set<TDbModel>().FirstOrDefaultAsync(m => m.Id == model.Id);  // Асинхронный поиск
            if (toUpdate != null)
            {
                Context.Entry(toUpdate).CurrentValues.SetValues(model);  // Обновление данных модели
                await Context.SaveChangesAsync();  // Асинхронное сохранение
            }
            return toUpdate;
        }

        public async Task<TDbModel> GetAsync(int id)
        {
            return await Context.Set<TDbModel>().FirstOrDefaultAsync(m => m.Id == id);  // Асинхронный поиск по Id
        }

        public Task<List<TDbModel>> GetAllAsync(TDbModel model)
        {
            throw new NotImplementedException();
        }
    }
}
