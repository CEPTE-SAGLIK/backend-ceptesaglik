using System;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.Business.DTOs
{
    public class NotificationCreateDto
    {
        [Required(ErrorMessage = "Kişi ID zorunludur.")]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "Bildirim başlığı zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bildirim mesajı zorunludur.")]
        public string Message { get; set; }
    }
}