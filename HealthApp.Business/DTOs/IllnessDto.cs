using System;

namespace HealthApp.Business.DTOs
{
    public class IllnessDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; } // Veri tabanı ilişkisi için kalıyor
        public string Name { get; set; }
        public DateTime DiagnosisDate { get; set; }
        public string Status { get; set; } // Flutter'daki "active", "recovered" vb. buraya gelecek
        public string? DoctorNotes { get; set; } // Flutter'daki isimle birebir aynı
    }
}