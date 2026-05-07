using HealthApp.Business.DTOs;
using HealthApp.Business.DTOs.Common;
using HealthApp.DataAccess;
using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthApp.Business.Services
{
    public class VaccineService
    {
        private readonly VaccineRepository _vaccineRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VaccineService(VaccineRepository vaccineRepository, IUnitOfWork unitOfWork)
        {
            _vaccineRepository = vaccineRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<VaccineDto>> CreateVaccineAsync(CreateVaccineDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                    return Result<VaccineDto>.Failure("Aşı adı boş olamaz.");

                string finalDescription = dto.Description ?? "";
                if (!string.IsNullOrEmpty(dto.Frequency))
                {
                    finalDescription = $"[Sıklık: {dto.Frequency}] " + finalDescription;
                }

                if (!DateTime.TryParse(dto.Date, out DateTime parsedDate))
                {
                    parsedDate = DateTime.Now;
                }

                var newVaccine = new Vaccine
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Date = parsedDate,
                    Description = finalDescription,
                    Status = (VaccineStatus)dto.Status,
                    CreatedAt = DateTime.Now,
                    VaccineScheduleId = null,
                    Dose = !string.IsNullOrWhiteSpace(dto.Dose) ? dto.Dose : "1. Doz"
                };

                // --- AKILLI ID EŞLEME MANTIĞI ---
                string targetIdStr = !string.IsNullOrEmpty(dto.ChildId) ? dto.ChildId : dto.PersonId;
                bool isLinked = false;

                if (Guid.TryParse(targetIdStr, out Guid targetId))
                {
                    var child = await _unitOfWork.GetRepository<Child>().GetByIdAsync(targetId);
                    if (child != null)
                    {
                        newVaccine.ChildId = targetId;
                        isLinked = true;
                    }
                    else
                    {
                        var person = await _unitOfWork.GetRepository<Person>().GetByIdAsync(targetId);
                        if (person != null)
                        {
                            newVaccine.PersonId = targetId;
                            isLinked = true;
                        }
                    }
                }

                if (!isLinked)
                {
                    return Result<VaccineDto>.Failure("Seçilen kişi veritabanında bulunamadı.");
                }

                var created = await _vaccineRepository.AddAsync(newVaccine);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = new VaccineDto(
                    created.Id,
                    created.Name ?? "Bilinmeyen Aşı",
                    created.Date,
                    created.Dose ?? "1. Doz",
                    created.Status.ToString(),
                    created.Description ?? "",
                    null,
                    null,
                    created.ChildId
                );

                return Result<VaccineDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<VaccineDto>.Failure("Aşı kaydedilirken hata oluştu: " + ex.Message);
            }
        }

        // --- GÜNCELLENEN SİLME METODU (ZİNCİRLEME SİLME DAHİL) ---
        public async Task<Result<bool>> DeleteVaccineAsync(Guid id)
        {
            try
            {
                var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(id);
                if (vaccine == null) return Result<bool>.Failure("Aşı bulunamadı.");

                // Zincirleme Silme: Bağlı hatırlatıcıyı bul ve temizle
                var reminderRepo = _unitOfWork.GetRepository<Reminder>();
                var allReminders = await reminderRepo.GetAllAsync();

                // RelatedItemId (VaccineId) ile eşleşen hatırlatıcıyı bul
                var relatedReminder = allReminders.FirstOrDefault(r => r.RelatedItemId == id.ToString());

                if (relatedReminder != null)
                {
                    await reminderRepo.DeleteAsync(relatedReminder);
                }

                await _unitOfWork.GetRepository<Vaccine>().DeleteAsync(vaccine);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure("Silme hatası: " + ex.Message);
            }
        }

        public async Task<Result<List<VaccineScheduleDto>>> GetSchedulesByChildIdAsync(Guid childId)
        {
            try
            {
                var vaccines = await _vaccineRepository.GetByChildIdAsync(childId);

                var schedule = new VaccineScheduleDto(
                    Guid.NewGuid(),
                    "Genel Takvim",
                    vaccines.Count(v => v.Status == VaccineStatus.Completed),
                    DateTime.Now,
                    vaccines.Select(v => new VaccineDto(
                        v.Id,
                        v.Name ?? "Bilinmeyen Aşı",
                        v.Date,
                        "1. Doz",
                        v.Status.ToString(),
                        v.Description ?? "",
                        null, null, v.ChildId)).ToList(),
                    vaccines.Count > 0 && vaccines.All(v => v.Status == VaccineStatus.Completed),
                    vaccines.Count(v => v.Status == VaccineStatus.Completed),
                    vaccines.Count(v => v.Status != VaccineStatus.Completed)
                );

                return Result<List<VaccineScheduleDto>>.Success(new List<VaccineScheduleDto> { schedule });
            }
            catch (Exception ex)
            {
                return Result<List<VaccineScheduleDto>>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<VaccineDto>>> GetByPersonIdAsync(Guid personId)
        {
            try
            {
                var vaccines = await _vaccineRepository.GetByPersonIdAsync(personId);

                var dtos = vaccines.Select(v => new VaccineDto(
                    v.Id,
                    v.Name ?? "Bilinmeyen Aşı",
                    v.Date,
                    v.Dose ?? "1. Doz",
                    v.Status.ToString(),
                    v.Description ?? "",
                    null, null, v.ChildId)).ToList();

                return Result<List<VaccineDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<VaccineDto>>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateStatusAsync(Guid id, string status)
        {
            try
            {
                var vaccine = await _unitOfWork.GetRepository<Vaccine>().GetByIdAsync(id);
                if (vaccine == null) return Result<bool>.Failure("Aşı bulunamadı.");

                if (Enum.TryParse<VaccineStatus>(status, true, out var newStatus))
                {
                    vaccine.Status = newStatus;
                    await _unitOfWork.SaveChangesAsync();
                    return Result<bool>.Success(true);
                }
                return Result<bool>.Failure("Geçersiz durum.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}