using Domain.Models;
using StudyProj.Repositories.Interfaces;

public interface IFacilityService : IBaseRepository<Facility>
{
    Task<List<Facility>> GetAllAsync(Facility facility);
}