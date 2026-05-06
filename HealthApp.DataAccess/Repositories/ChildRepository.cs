using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace HealthApp.DataAccess.Repositories
{
    public class ChildRepository : GenericRepository<Child>, IChildRepository
    {
        public ChildRepository(AppDbContext context) : base(context) { }

        public async Task<Child?> GetWithVaccineScheduleAsync(Guid id)
        {
            // Tek bir çocuğun takvimini ve aşılarını tam detaylı getirir
            return await _dbSet
                .Include(c => c.VaccineSchedules)
                    .ThenInclude(vs => vs.Vaccines)
                .Include(c => c.Vaccines)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Child>> GetByUserIdAsync(Guid userId)
        {
            // Sadece giriş yapan kullanıcıya ait çocukları ve aşı bilgilerini getirir
            return await _dbSet
                .Include(c => c.VaccineSchedules)
                    .ThenInclude(vs => vs.Vaccines)
                .Include(c => c.Vaccines)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Child>> GetAllWithSchedulesAsync()
        {
            return await _dbSet
                .Include(c => c.Vaccines) // Buraya da ekledik
                .Include(c => c.VaccineSchedules)
                    .ThenInclude(vs => vs.Vaccines)
                .ToListAsync();
        }
    }
}