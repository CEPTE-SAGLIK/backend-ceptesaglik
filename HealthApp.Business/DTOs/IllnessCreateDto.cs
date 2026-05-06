using System;

namespace HealthApp.Business.DTOs
{
    // burada Id yok, çünkü yeni bir hastalık eklerken ID'yi biz değil, veri tabanı otomatik oluşturur.
    public class IllnessCreateDto
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }
        public DateTime DiagnosisDate { get; set; }

        // Eski Description ve IsChronic silindi, yerlerine yenileri geldi
        public string Status { get; set; }
        public string? DoctorNotes { get; set; }
    }
}