using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Domain.Entities
{
    public class Vaccine : BaseEntity
    {
        public Guid? VaccineScheduleId { get; set; }
        public Guid? ChildId { get; set; }
        public Guid? PersonId { get; set; } // KRİTİK: Yetişkin aşılarını bağlamak için eklendi

        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Dose { get; set; } = string.Empty;
        public HealthApp.Domain.Enums.VaccineStatus Status { get; set; } = HealthApp.Domain.Enums.VaccineStatus.Pending;
        public string? Description { get; set; }
        public DateTime? NextDoseDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Navigation Properties
        public virtual VaccineSchedule? VaccineSchedule { get; set; }
        public virtual Child? Child { get; set; }
        public virtual Person? Person { get; set; } // İlişki tanımlandı
    }
}