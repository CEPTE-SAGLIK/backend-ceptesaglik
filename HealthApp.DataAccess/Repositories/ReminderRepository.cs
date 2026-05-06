using HealthApp.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;

namespace HealthApp.DataAccess.Repositories
{
    public class ReminderRepository : GenericRepository<Reminder>, IReminderRepository
    {
        public ReminderRepository(AppDbContext context) : base(context)
        {
        }
    }
}