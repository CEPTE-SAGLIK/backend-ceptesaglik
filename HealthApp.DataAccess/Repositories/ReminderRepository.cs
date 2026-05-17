using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.DataAccess.Repositories
{
    public class ReminderRepository : GenericRepository<Reminder>, IReminderRepository
    {
        public ReminderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Reminder>> GetByVaccineIdAsync(Guid vaccineId)
        {
            return await _dbSet.Where(r => r.VaccineId == vaccineId).ToListAsync();
        }

        public async Task<List<Reminder>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet.Where(r => r.PersonId == personId).ToListAsync();
        }
    }
}