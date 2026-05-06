using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Repositories;
using HealthApp.Business.DTOs;

namespace HealthApp.Business.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository _repository;

        public NotificationService(NotificationRepository repository)
        {
            _repository = repository;
        }

        // 1. Yeni Bildirim Ekleme
        public async Task<NotificationDto> AddNotificationAsync(NotificationCreateDto createDto)
        {
            var notification = new Notification
            {
                PersonId = createDto.PersonId,
                Title = createDto.Title,
                Message = createDto.Message,
                IsRead = false // Yeni gelen bildirim varsayılan olarak okunmamıştır
            };

            await _repository.AddAsync(notification);

            return new NotificationDto
            {
                Id = notification.Id,
                PersonId = notification.PersonId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }

        // 2. Tüm Bildirimleri Getirme
        public async Task<List<NotificationDto>> GetAllNotificationsAsync()
        {
            var notifications = await _repository.GetAllAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                PersonId = n.PersonId,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }
    }
}