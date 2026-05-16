using System;
using HealthApp.Domain.Enums;

namespace HealthApp.Domain.Entities
{
    public class Person : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }

        // Hesap sahibinin kendi profili mi? (onboarding'de oluşturulan profil = true,
        // uygulamadan eklenen aile üyeleri = false)
        public bool IsAccountOwner { get; set; } = false;

        // Profil arayüzüne eklediğimiz Kronik Hastalıklar ve Alerjiler
        public List<string> ChronicDiseases { get; set; } = new List<string>();
        public List<string> Allergies { get; set; } = new List<string>();

        public User User { get; set; } = null!; // foreign key relationship to User entity

        public virtual ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
    }
}
