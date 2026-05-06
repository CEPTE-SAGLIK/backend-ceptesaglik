using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Repositories;

namespace HealthApp.Business.Services
{
    public interface IReminderService
    {
        Task<List<Reminder>> GetRemindersByUserIdAsync(Guid userId);
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task UpdateReminderAsync(Reminder reminder);
        Task DeleteReminderAsync(Guid id);
    }

    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _repository;

        public ReminderService(IReminderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Reminder>> GetRemindersByUserIdAsync(Guid userId)
        {
            var all = await _repository.GetAllAsync();
            return all.Where(x => x.UserId == userId)
                      .OrderBy(x => x.ReminderDate)
                      .ToList();
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            await _repository.AddAsync(reminder);
            return reminder;
        }

        public async Task UpdateReminderAsync(Reminder reminder)
        {
            await _repository.UpdateAsync(reminder);
        }

        public async Task DeleteReminderAsync(Guid id)
        {
            var reminder = await _repository.GetByIdAsync(id);
            if (reminder != null)
            {
                await _repository.DeleteAsync(reminder);
            }
        }
    }
}