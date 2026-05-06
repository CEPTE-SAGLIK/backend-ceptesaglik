using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Domain.Entities
{
    public class Medicine : BaseEntity
    {
        public Guid UserId { get; set; } // Hangi kullanıcıya ait?
        public string Name { get; set; } = string.Empty; // İlaç Adı

        // Mevcut Alanlar (Geriye dönük uyumluluk için korundu)
        public string Frequency { get; set; } = string.Empty; // Sıklık
        public string Time { get; set; } = string.Empty; // Saat
        public string? Notes { get; set; } // Ek Notlar
        public bool IsActive { get; set; } = true; // İlaç kullanımı devam ediyor mu?

        // Flutter (medicine.dart) Modelinden Gelen Yeni Alanlar
        public string? UsageInstructions { get; set; } // Kullanım talimatları
        public int TimesPerDay { get; set; } = 1; // Günde kaç kez
        public DateTime StartDate { get; set; } = DateTime.Now; // Başlangıç tarihi
        public DateTime? EndDate { get; set; } // Bitiş tarihi


        // İlişkiler
        public virtual User? User { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
