using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.DataAccess.Repositories
{
    public class MedicineRepository : GenericRepository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Medicine>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet.Where(m => m.PersonId == personId).ToListAsync();
        }
    }
}
