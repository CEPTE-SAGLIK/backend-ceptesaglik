using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.DataAccess.Context;
using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore; // ToListAsync için gerekli

namespace HealthApp.DataAccess.Repositories
{
    public class VaccineRepository : GenericRepository<Vaccine>
    {
        public VaccineRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Vaccine>> GetByChildIdAsync(Guid childId)
        {
            return await _dbSet
                .Where(v => v.ChildId == childId)
                .ToListAsync();
        }

        // KRİTİK: CS1061 hatasını çözen yeni metod
        public async Task<List<Vaccine>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet
                .Where(v => v.PersonId == personId)
                .ToListAsync();
        }
    }
}