using System;

namespace HealthApp.Domain.Entities
{
    // BaseEntity bağlantısını koruduk! (Id muhtemelen oradan geliyor)
    public class Allergy : BaseEntity
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }
        public string? Level { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}