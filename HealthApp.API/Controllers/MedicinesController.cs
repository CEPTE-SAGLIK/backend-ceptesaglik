using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthApp.Business.Services;
using HealthApp.Business.DTOs;
using HealthApp.DataAccess;
using HealthApp.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace HealthApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly MedicineService _medicineService;
        private readonly IUnitOfWork _unitOfWork;

        public MedicinesController(MedicineService medicineService, IUnitOfWork unitOfWork)
        {
            _medicineService = medicineService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicineDto dto)
        {
            var result = await _medicineService.CreateMedicineAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

            try
            {
                var medicineId = result.Data;
                var medicine = await _unitOfWork.GetRepository<Medicine>().GetByIdAsync(medicineId);

                if (medicine != null)
                {
                    // İlacın kendi tablosundaki Frequency kolonunu metin olarak kaydediyoruz.
                    medicine.Frequency = dto.RepeatType.ToString();
                    medicine.TimesPerDay = dto.TimesPerDay;

                    var reminder = new Reminder
                    {
                        Id = Guid.NewGuid(),
                        UserId = medicine.UserId,
                        Title = medicine.Name,
                        Description = "İlaç Hatırlatıcısı: " + (medicine.UsageInstructions ?? ""),
                        Type = (ReminderType)0, // ReminderType.Medicine
                        ReminderDate = DateTime.Now,
                        MedicineId = medicine.Id,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        RepeatType = (RepeatType)dto.RepeatType
                    };

                    await _unitOfWork.GetRepository<Reminder>().AddAsync(reminder);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--- [HATIRLATICI HATASI] Oluşturma başarısız: {ex.Message} ---");
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var remindersRepo = _unitOfWork.GetRepository<Reminder>();
                var relatedReminders = await remindersRepo.GetAllAsync();
                var toDelete = relatedReminders.Where(r => r.MedicineId == id).ToList();

                foreach (var reminder in toDelete)
                {
                    await remindersRepo.DeleteAsync(reminder);
                }

                await _unitOfWork.SaveChangesAsync();

                var result = await _medicineService.DeleteMedicineAsync(id);
                if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Silme işlemi sırasında hata oluştu: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateMedicineDto dto)
        {
            // 1. Temel ilaç bilgilerini güncelle
            var result = await _medicineService.UpdateMedicineAsync(id, dto);
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

            try
            {
                var medicine = await _unitOfWork.GetRepository<Medicine>().GetByIdAsync(id);
                if (medicine != null)
                {
                    // 2. İlacın Frequency bilgisini güncelle (İlaçlarım sekmesi için)
                    medicine.Frequency = dto.RepeatType.ToString();
                    medicine.TimesPerDay = dto.TimesPerDay;

                    // 3. Hatırlatıcıyı (Takvimi) güncelle
                    var remindersRepo = _unitOfWork.GetRepository<Reminder>();
                    var allReminders = await remindersRepo.GetAllAsync();
                    var relatedReminder = allReminders.FirstOrDefault(r => r.MedicineId == id);

                    if (relatedReminder != null)
                    {
                        // Var olan hatırlatıcıyı güncelle
                        relatedReminder.RepeatType = (RepeatType)dto.RepeatType;
                        relatedReminder.Title = medicine.Name;
                        relatedReminder.Description = "İlaç Hatırlatıcısı: " + (medicine.UsageInstructions ?? "");
                    }
                    else
                    {
                        // EĞER YOKSA: Yeni bir hatırlatıcı oluştur (Senkronizasyonu sağlar)
                        var newReminder = new Reminder
                        {
                            Id = Guid.NewGuid(),
                            UserId = medicine.UserId,
                            Title = medicine.Name,
                            Description = "İlaç Hatırlatıcısı: " + (medicine.UsageInstructions ?? ""),
                            Type = (ReminderType)0,
                            ReminderDate = DateTime.Now,
                            MedicineId = medicine.Id,
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            RepeatType = (RepeatType)dto.RepeatType
                        };
                        await remindersRepo.AddAsync(newReminder);
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--- [GÜNCELLEME HATASI] Takvim senkronize edilemedi: {ex.Message} ---");
            }

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _medicineService.GetAllMedicinesAsync();
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _medicineService.GetMedicinesByUserIdAsync(userId);
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }
    }
}