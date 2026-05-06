using System;

namespace HealthApp.Domain.Entities
{
    // BaseEntity bağlantısını koruduk! (Id muhtemelen oradan geliyor)
    public class Allergy : BaseEntity
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        // Not: Eğer BaseEntity'nin içinde zaten bir CreatedDate varsa, bu satırın altını çizebilir. Öyle bir durum olursa bu satırı silebilirsin.
    }
}