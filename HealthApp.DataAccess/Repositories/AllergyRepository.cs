using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthApp.DataAccess.Repositories
{
    // GenericRepository'den miras alarak CRUD işlemlerini hazır getiriyoruz
    public class AllergyRepository : GenericRepository<Allergy>
    {
        public AllergyRepository(AppDbContext context) : base(context)
        {
        }

        // Seçilen kişiye (PersonId) ait alerjileri getiren özel metot
        public async Task<List<Allergy>> GetByPersonIdAsync(Guid personId)
        {
            return await _dbSet.Where(a => a.PersonId == personId).ToListAsync();
        }
    }
}