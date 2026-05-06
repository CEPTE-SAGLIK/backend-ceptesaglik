using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.DataAccess.Repositories
{
    public class PersonRepository : GenericRepository<Person>
    {
        public PersonRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Person>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Person?> GetByIdAndUserIdAsync(Guid personId, Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Id == personId && p.UserId == userId);
        }
    }
}
