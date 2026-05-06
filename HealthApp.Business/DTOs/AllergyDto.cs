using System;

namespace HealthApp.Business.DTOs
{
    public class AllergyDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; } // Veri tabanı ilişkisi için kalıyor
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Level { get; set; }
        public string? Description { get; set; }
    }
}