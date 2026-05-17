using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Repositories;
using HealthApp.DataAccess;
using HealthApp.Business.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HealthApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemindersController : ControllerBase
    {
        private readonly IReminderRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RemindersController(IReminderRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        // GET /api/reminders/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            try
            {
                var allReminders = await _repository.GetAllAsync();
                var userIdStr = userId.ToString().ToLower();

                var reminders = allReminders
                                .Where(x => x.UserId.ToString().ToLower() == userIdStr)
                                .ToList();

                var children = (await _unitOfWork.GetRepository<Child>().GetAllAsync())
                               .Where(c => c.UserId.ToString().ToLower() == userIdStr)
                               .ToList();
                var childrenIds = children.Select(c => c.Id).ToList();

                var medicines = (await _unitOfWork.GetRepository<Medicine>().GetAllAsync())
                                .Where(x => x.UserId.ToString().ToLower() == userIdStr)
                                .ToList();

                var vaccines = (await _unitOfWork.GetRepository<Vaccine>().GetAllAsync())
                               .Where(x => x.ChildId.HasValue && childrenIds.Contains(x.ChildId.Value))
                               .ToList();

                foreach (var med in medicines)
                {
                    if (!reminders.Any(r => r.MedicineId == med.Id))
                    {
                        var baseDate = med.StartDate.Date;
                        var reminderTime = baseDate;
                        if (!string.IsNullOrEmpty(med.Time) && TimeSpan.TryParse(med.Time, out var ts))
                            reminderTime = baseDate.Add(ts);

                        // Frequency string'inden tekrar tipini türet
                        var medRepeatType = med.Frequency switch
                        {
                            "Tek Seferlik" => RepeatType.None,
                            "Haftada Bir"  => RepeatType.Weekly,
                            "Aylık"        => RepeatType.Monthly,
                            _              => RepeatType.Daily,
                        };

                        reminders.Add(new Reminder
                        {
                            Id = med.Id,
                            Title = med.Name,
                            Description = med.UsageInstructions ?? "İlaç Hatırlatıcısı",
                            Type = ReminderType.Medicine,
                            ReminderDate = reminderTime,
                            RepeatType = medRepeatType,
                            AudienceGroup = med.AudienceGroup,
                            AudienceBirthDate = med.AudienceBirthDate,
                            UserId = userId,
                            IsActive = true,
                            MedicineId = med.Id
                        });
                    }
                }

                foreach (var vac in vaccines)
                {
                    if (!reminders.Any(r => r.VaccineId == vac.Id))
                    {
                        var vacChild = children.FirstOrDefault(c => c.Id == vac.ChildId);
                        reminders.Add(new Reminder
                        {
                            Id = vac.Id,
                            Title = vac.Name,
                            Description = vac.Dose ?? "Aşı Takvimi",
                            Type = ReminderType.Vaccine,
                            ReminderDate = vac.Date,
                            AudienceGroup = AudienceGroup.Child,
                            AudienceBirthDate = vacChild?.BirthDate,
                            UserId = userId,
                            IsActive = true,
                            IsCompleted = vac.Status.ToString() == "Tamamlandı",
                            VaccineId = vac.Id
                        });
                    }
                }

                var dtos = reminders
                    .OrderBy(x => x.ReminderDate)
                    .Select(r => ToDto(r))
                    .ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST /api/reminders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReminderDto dto)
        {
            try
            {
                if (dto == null) return BadRequest(new { error = "İstek gövdesi boş." });

                if (!Guid.TryParse(dto.UserId, out var userId))
                    return BadRequest(new { error = "Geçersiz UserId formatı." });

                var reminder = new Reminder
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = dto.Title,
                    Description = dto.Description,
                    ReminderDate = dto.ReminderDate,
                    Type = (ReminderType)dto.Type,
                    RepeatType = (RepeatType)dto.RepeatType,
                    AudienceGroup = (AudienceGroup)dto.AudienceGroup,
                    AudienceBirthDate = dto.AudienceBirthDate,
                    IsActive = dto.IsActive,
                    RelatedItemId = dto.RelatedItemId,
                    MedicineId = Guid.TryParse(dto.MedicineId, out var mid) ? mid : (Guid?)null,
                    VaccineId = Guid.TryParse(dto.VaccineId, out var vid) ? vid : (Guid?)null,
                    PersonId = Guid.TryParse(dto.TargetPersonId, out var pid) ? pid : (Guid?)null,
                };

                await _repository.AddAsync(reminder);
                return Ok(ToDto(reminder));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT /api/reminders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateReminderDto dto)
        {

            try
            {
            if (dto == null) return BadRequest(new { error = "İstek gövdesi boş." });

            var reminder = await _repository.GetByIdAsync(id);
            if (reminder == null) return NotFound(new { error = "Hatırlatıcı bulunamadı." });

            reminder.Title = dto.Title;
            reminder.Description = dto.Description;
            reminder.ReminderDate = dto.ReminderDate;
            reminder.Type = (ReminderType)dto.Type;
            reminder.RepeatType = (RepeatType)dto.RepeatType;
            reminder.AudienceGroup = (AudienceGroup)dto.AudienceGroup;
            reminder.AudienceBirthDate = dto.AudienceBirthDate;
            reminder.IsActive = dto.IsActive;
            reminder.RelatedItemId = dto.RelatedItemId;
            reminder.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(reminder);
            return Ok(ToDto(reminder));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE /api/reminders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var reminder = await _repository.GetByIdAsync(id);

                if (reminder != null)
                {
                    if (reminder.MedicineId.HasValue)
                    {
                        var med = await _unitOfWork.GetRepository<Medicine>().GetByIdAsync(reminder.MedicineId.Value);
                        if (med != null) await _unitOfWork.GetRepository<Medicine>().DeleteAsync(med);
                    }

                    if (reminder.VaccineId.HasValue)
                    {
                        var vac = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(reminder.VaccineId.Value);
                        if (vac != null) await _unitOfWork.GetRepository<Vaccine>().DeleteAsync(vac);
                    }

                    await _repository.DeleteAsync(reminder);
                }
                else
                {
                    var med = await _unitOfWork.GetRepository<Medicine>().GetByIdAsync(id);
                    if (med != null)
                    {
                        var relatedReminders = (await _repository.GetAllAsync()).Where(r => r.MedicineId == id);
                        foreach (var r in relatedReminders) await _repository.DeleteAsync(r);
                        await _unitOfWork.GetRepository<Medicine>().DeleteAsync(med);
                    }
                    else
                    {
                        var vac = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(id);
                        if (vac != null)
                        {
                            var relatedReminders = (await _repository.GetAllAsync()).Where(r => r.VaccineId == id);
                            foreach (var r in relatedReminders) await _repository.DeleteAsync(r);
                            await _unitOfWork.GetRepository<Vaccine>().DeleteAsync(vac);
                        }
                        else return NotFound(new { error = "Kayıt bulunamadı" });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private static ReminderDto ToDto(Reminder r) => new ReminderDto
        {
            Id = r.Id.ToString(),
            UserId = r.UserId.ToString(),
            Title = r.Title,
            Description = r.Description,
            DateTime = r.ReminderDate.ToString("o"),
            Type = r.Type switch
            {
                ReminderType.Medicine => "medicine",
                ReminderType.Vaccine => "vaccine",
                ReminderType.Appointment => "appointment",
                ReminderType.Custom => "custom",
                _ => "custom"
            },
            RepeatType = r.RepeatType switch
            {
                Domain.Entities.RepeatType.Daily => "daily",
                Domain.Entities.RepeatType.Weekly => "weekly",
                Domain.Entities.RepeatType.Monthly => "monthly",
                _ => "none"
            },
            AudienceGroup = DeriveAudience(r.AudienceBirthDate, r.AudienceGroup) switch
            {
                Domain.Entities.AudienceGroup.Elderly => "elderly",
                Domain.Entities.AudienceGroup.Child => "child",
                _ => "adult"
            },
            AudienceBirthDate = r.AudienceBirthDate?.ToString("o"),
            IsActive = r.IsActive,
            IsCompleted = r.IsCompleted,
            RelatedItemId = r.RelatedItemId,
            MedicineId = r.MedicineId?.ToString(),
            VaccineId = r.VaccineId?.ToString(),
            TargetPersonId = r.PersonId?.ToString()
        };

        // Doğum tarihinden yaş grubunu dinamik hesaplar; tarih yoksa kayıtlı gruba düşer.
        // Eşikler: çocuk < 18, yaşlı >= 65, arası yetişkin.
        private static AudienceGroup DeriveAudience(DateTime? birthDate, AudienceGroup fallback)
        {
            if (birthDate == null) return fallback;
            var today = DateTime.UtcNow.Date;
            var bd = birthDate.Value.Date;
            var age = today.Year - bd.Year;
            if (bd > today.AddYears(-age)) age--;
            if (age < 18) return AudienceGroup.Child;
            if (age >= 65) return AudienceGroup.Elderly;
            return AudienceGroup.Adult;
        }
    }
}
