using System;

namespace HealthApp.Business.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } // Bildirimin ne zaman geldiği UI için önemli
    }
}