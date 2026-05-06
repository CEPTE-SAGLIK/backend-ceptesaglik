using System;

namespace HealthApp.Domain.Entities
{
    public class Illness : BaseEntity
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string Status { get; set; }
        public string? DoctorNotes { get; set; }
    }
}