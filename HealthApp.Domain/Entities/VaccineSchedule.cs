using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Domain.Entities
{
    public class VaccineSchedule : BaseEntity
    {
        public Guid ChildId { get; set; }
        public string Period { get; set; } = string.Empty;      // "Doğumda", "1. Ay Sonu" vb.
        public int MonthIndex { get; set; }                     // 0, 1, 2, 4, 6, 12...
        public DateTime ScheduledDate { get; set; }

        // Navigation Properties
        public virtual Child Child { get; set; } = null!;
        public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>(); // Koleksiyon ismi çoğul
    }
}