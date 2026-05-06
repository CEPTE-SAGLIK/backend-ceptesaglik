using HealthApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthApp.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}