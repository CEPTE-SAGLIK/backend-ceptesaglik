using System;

namespace HealthApp.Domain.Entities
{
    // BaseEntity'den miras alarak Id ve CreatedAt gibi özellikleri otomatik kazanıyoruz
    public class Notification : BaseEntity
    {
        public Guid PersonId { get; set; } // Bildirimin gideceği kişi
        public string Title { get; set; } // Bildirim başlığı (Örn: "İlaç Vakti!")
        public string Message { get; set; } // Bildirim içeriği
        public bool IsRead { get; set; } = false; // Okundu mu? (Varsayılan olarak hayır)
    }
}