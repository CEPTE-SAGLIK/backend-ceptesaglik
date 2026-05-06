using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;

namespace HealthApp.DataAccess.Repositories
{
    public class MedicineRepository : GenericRepository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(AppDbContext context) : base(context)
        {
        }
    }
}
