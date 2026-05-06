using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Context; // <-- EKSİK OLAN SATIR BUYDU
using Microsoft.EntityFrameworkCore;

namespace HealthApp.DataAccess.Repositories
{
    public class IllnessRepository : GenericRepository<Illness>
    {
        public IllnessRepository(AppDbContext context) : base(context)
        {
        }

        // Seçilen kişiye (PersonId) ait hastalıkları getiren özel metot
        public async Task<List<Illness>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet.Where(i => i.PersonId == personId).ToListAsync();
        }
    }
}