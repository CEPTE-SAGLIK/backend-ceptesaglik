using System.ComponentModel.DataAnnotations.Schema;
using HealthApp.Domain.Entities;

namespace HealthApp.Domain.Entities
{
    public class Reminder : BaseEntity
    {
        // 1. User Bağlantısı (Kalıcı - Bozulmadı)
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ReminderDate { get; set; }
        public ReminderType Type { get; set; }
        public RepeatType RepeatType { get; set; } = RepeatType.None;
        public AudienceGroup AudienceGroup { get; set; } = AudienceGroup.Adult;
        // Hedef kişinin doğum tarihi — yaş grubu bundan dinamik hesaplanır (snapshot değil).
        // Null ise AudienceGroup alanına geri düşülür.
        public DateTime? AudienceBirthDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // 2. İlaç Bağlantısı (Yeni ve Güncel)
        public Guid? MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual Medicine? Medicine { get; set; }

        // 3. Aşı Bağlantısı (Yeni ve Güncel)
        public Guid? VaccineId { get; set; }
        [ForeignKey("VaccineId")]
        public virtual Vaccine? Vaccine { get; set; }

        // Hangi aile üyesine ait (null = hesap sahibinin kendisi)
        public Guid? PersonId { get; set; }

        public string? RelatedItemId { get; set; }
    }

    // ALTIN KURAL: Bu tanımlar burada durmalı ki CS0246 ve CS1061 hataları gitsin.
    public enum ReminderType
    {
        Medicine = 0,
        Vaccine = 1,
        Appointment = 2,
        Custom = 3
    }

    public enum RepeatType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    // Hatırlatıcının hedef yaş grubu (yetişkin / yaşlı / çocuk).
    public enum AudienceGroup
    {
        Adult = 0,
        Elderly = 1,
        Child = 2
    }
}