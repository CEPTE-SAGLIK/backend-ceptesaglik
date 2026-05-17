using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.DataAccess.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Notification>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet.Where(n => n.PersonId == personId).ToListAsync();
        }
    }
}