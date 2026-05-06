using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;

namespace HealthApp.DataAccess.Repositories
{
    public interface IChildRepository : IGenericRepository<Child>
    {
        // Çocukları aşılarıyla birlikte getiren özel metot
        Task<List<Child>> GetAllWithSchedulesAsync();
        Task<Child?> GetWithVaccineScheduleAsync(Guid id);
    }
}
