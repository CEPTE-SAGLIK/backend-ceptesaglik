using System;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Business.DTOs
{
    public class AllergyCreateDto
    {
        [Required]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "Alerji adı zorunludur.")]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? Level { get; set; }       // "mild", "moderate", "severe"
        public string? Description { get; set; }
    }
}