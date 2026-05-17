using System.ComponentModel.DataAnnotations;

namespace HealthApp.Business.DTOs
{
    public class AllergyUpdateDto
    {
        [Required(ErrorMessage = "Alerji adı zorunludur.")]
        public string Name { get; set; }

        public string? Level { get; set; }
        public string? Description { get; set; }
    }
}
