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
    public class VaccineScheduleRepository : GenericRepository<VaccineSchedule>
    {
        public VaccineScheduleRepository(AppDbContext context) : base(context) { }

        public async Task<List<VaccineSchedule>> GetSchedulesByChildIdAsync(Guid childId)
        {
            return await _dbSet
                // HATA DÜZELTİLDİ: Vaccine -> Vaccines (Teknik doküman uyumu)
                .Include(vs => vs.Vaccines)
                .Where(vs => vs.ChildId == childId)
                // HATA DÜZELTİLDİ: PlannedDate -> MonthIndex veya ScheduledDate
                .OrderBy(vs => vs.MonthIndex)
                .ToListAsync();
        }

        public async Task<List<VaccineSchedule>> GetPendingSchedulesAsync(Guid childId)
        {
            return await _dbSet
                .Include(vs => vs.Vaccines) // Plural sync
                .Where(vs => vs.ChildId == childId && vs.Vaccines.Any(v => v.Status != HealthApp.Domain.Enums.VaccineStatus.Completed))
                .OrderBy(vs => vs.ScheduledDate)
                .ToListAsync();
        }
    }
}