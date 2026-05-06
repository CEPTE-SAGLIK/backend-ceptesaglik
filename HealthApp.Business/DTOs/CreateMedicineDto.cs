using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    public class CreateMedicineDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? UsageInstructions { get; set; }
        public int TimesPerDay { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RepeatType { get; set; }
    }
}
