using System;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Business.DTOs
{
    public class UpdateChildDto
    {
        [Required(ErrorMessage = "Çocuk adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
    }
}
