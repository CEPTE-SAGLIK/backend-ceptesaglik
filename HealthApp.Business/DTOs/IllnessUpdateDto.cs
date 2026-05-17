using System;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Business.DTOs
{
    public class IllnessUpdateDto
    {
        [Required(ErrorMessage = "Hastalık adı zorunludur.")]
        public string Name { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? DoctorNotes { get; set; }
        public DateTime? DiagnosisDate { get; set; }
    }
}
