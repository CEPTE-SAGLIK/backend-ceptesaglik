using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.DataAccess.Context;
using HealthApp.DataAccess;
using HealthApp.Domain.Entities;
using HealthApp.Business.DTOs;
using HealthApp.Business.DTOs.Common;

namespace HealthApp.Business.Services
{
    public class MedicineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Tüm ilaçları listeler
        /// </summary>
        public async Task<Result<List<Medicine>>> GetAllMedicinesAsync()
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Medicine>();
                var medicines = await repository.GetAllAsync();

                return Result<List<Medicine>>.Success(medicines.ToList());
            }
            catch (Exception ex)
            {
                return Result<List<Medicine>>.Failure($"İlaçlar listelenirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Belirli bir kullanıcıya ait ilaçları listeler
        /// </summary>
        public async Task<Result<List<Medicine>>> GetMedicinesByUserIdAsync(Guid userId)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Medicine>();
                var allMedicines = await repository.GetAllAsync();

                var userMedicines = allMedicines.Where(m => m.UserId == userId).ToList();

                return Result<List<Medicine>>.Success(userMedicines);
            }
            catch (Exception ex)
            {
                return Result<List<Medicine>>.Failure($"Kullanıcıya ait ilaçlar getirilemedi: {ex.Message}");
            }
        }

        /// <summary>
        /// İlacı veritabanından siler
        /// </summary>
        public async Task<Result<bool>> DeleteMedicineAsync(Guid id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Medicine>();
                var medicine = await repository.GetByIdAsync(id);

                if (medicine == null)
                    return Result<bool>.Failure("Silinecek ilaç bulunamadı.");

                // DÜZELTİLDİ: IGenericRepository içindeki 'DeleteAsync' metodunu çağırdık ve await ekledik.
                await repository.DeleteAsync(medicine);

                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İlaç silinirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Yeni ilaç kaydı oluşturur
        /// </summary>
        public async Task<Result<Guid>> CreateMedicineAsync(CreateMedicineDto dto)
        {
            try
            {
                var medicine = new Medicine
                {
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Frequency = dto.Frequency,
                    Time = dto.Time,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    UsageInstructions = dto.UsageInstructions,
                    TimesPerDay = dto.TimesPerDay,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    AudienceGroup = (AudienceGroup)dto.AudienceGroup,
                    AudienceBirthDate = dto.AudienceBirthDate,
                    PersonId = dto.PersonId
                };

                var repository = _unitOfWork.GetRepository<Medicine>();
                await repository.AddAsync(medicine);

                await _unitOfWork.SaveChangesAsync();

                return Result<Guid>.Success(medicine.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"İlaç kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Mevcut bir ilacı günceller
        /// </summary>
        public async Task<Result<bool>> UpdateMedicineAsync(Guid id, CreateMedicineDto dto)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<Medicine>();
                var medicine = await repository.GetByIdAsync(id);

                if (medicine == null)
                    return Result<bool>.Failure("Güncellenecek ilaç bulunamadı.");

                // Alanları DTO'dan gelen verilerle güncelliyoruz
                medicine.Name = dto.Name;
                medicine.Frequency = dto.Frequency;
                medicine.Time = dto.Time;
                medicine.Notes = dto.Notes;
                medicine.UsageInstructions = dto.UsageInstructions;
                medicine.TimesPerDay = dto.TimesPerDay;
                medicine.StartDate = dto.StartDate;
                medicine.EndDate = dto.EndDate;
                medicine.AudienceGroup = (AudienceGroup)dto.AudienceGroup;
                medicine.AudienceBirthDate = dto.AudienceBirthDate;
                medicine.UpdatedAt = DateTime.Now;

                await repository.UpdateAsync(medicine);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İlaç güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}