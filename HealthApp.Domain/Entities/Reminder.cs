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
}