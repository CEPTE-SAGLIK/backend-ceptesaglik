using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;

namespace HealthApp.DataAccess.Repositories
{
    // O sihirli GenericRepository sayesinde tüm ekleme/silme/getirme kodları otomatik geliyor!
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }
    }
}