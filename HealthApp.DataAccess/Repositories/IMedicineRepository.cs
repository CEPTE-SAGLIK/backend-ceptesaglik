using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;

namespace HealthApp.DataAccess.Repositories
{
    public interface IMedicineRepository : IGenericRepository<Medicine>
    {
    }
}
