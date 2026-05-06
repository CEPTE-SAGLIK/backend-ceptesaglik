using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace HealthApp.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync();
        IMedicineRepository Medicines { get; }
    }
}