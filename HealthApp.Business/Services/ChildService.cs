// HealthApp.Business/Services/ChildService.cs
using AutoMapper;
using HealthApp.Business.DTOs;
using HealthApp.Business.DTOs.Common;
using HealthApp.DataAccess;
using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HealthApp.Business.Services
{
    public class ChildService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly VaccineScheduleGenerator _scheduleGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChildRepository _childRepository; // Doğrudan repository ekledik

        public ChildService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            VaccineScheduleGenerator scheduleGenerator,
            IHttpContextAccessor httpContextAccessor,
            ChildRepository childRepository) // Enjeksiyon yapıldı
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _scheduleGenerator = scheduleGenerator;
            _httpContextAccessor = httpContextAccessor;
            _childRepository = childRepository;
        }

        // 1. Yeni Çocuk Oluşturma (Görseldeki Line 62 hatasını çözer)
        public async Task<Result<ChildDto>> CreateAsync(Guid userId, CreateChildDto dto)
        {
            try
            {
                var child = _mapper.Map<Child>(dto);
                child.UserId = userId;
                child.Id = Guid.NewGuid();

                // Otomatik aşı takvimi üretimi aktifleştirildi
                child.VaccineSchedules = _scheduleGenerator.Generate(child.Id, child.BirthDate);

                await _childRepository.AddAsync(child);
                await _unitOfWork.SaveChangesAsync();
                return Result<ChildDto>.Success(_mapper.Map<ChildDto>(child));
            }
            catch (Exception ex) { return Result<ChildDto>.Failure(ex.Message); }
        }

        // 2. Çocuk Kaydı Silme (Görseldeki Line 71 hatasını çözer)
        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Child>();

                // Sadece çocuğu değil, ilişkili tabloları da içerecek şekilde getiriyoruz
                var child = await _childRepository.GetWithVaccineScheduleAsync(id);

                if (child == null) return Result<bool>.Failure("Çocuk bulunamadı.");

                await repo.DeleteAsync(child);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex) { return Result<bool>.Failure("Silme hatası: " + ex.Message); }
        }

        // 3. Fiziksel Bilgileri Güncelleme (Görseldeki Line 85 hatasını çözer)
        public async Task<Result<ChildDto>> UpdatePhysicalInfoAsync(Guid id, double height, double weight, string gender)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Child>();
                var child = await repo.GetByIdAsync(id);
                if (child == null) return Result<ChildDto>.Failure("Çocuk bulunamadı.");

                // EKSİK OLAN KISIM: Boy ve kilo atamaları eklendi
                child.Height = height;
                child.Weight = weight;

                // Enum dönüşümü
                if (Enum.TryParse<Gender>(gender, true, out var genderEnum))
                {
                    child.Gender = genderEnum;
                }

                await repo.UpdateAsync(child);
                await _unitOfWork.SaveChangesAsync();
                return Result<ChildDto>.Success(_mapper.Map<ChildDto>(child));
            }
            catch (Exception ex) { return Result<ChildDto>.Failure(ex.Message); }
        }

        // --- MEVCUT DİĞER METOTLARIN (Aşağısı değişmedi) ---

        public async Task<Result<List<ChildDto>>> GetAllAsync()
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Result<List<ChildDto>>.Failure("Yetkisiz erişim.");

                // 400 Hatasının çözümü: Cast yapmak yerine doğrudan enjekte edilen repository'yi kullanıyoruz
                var children = await _childRepository.GetByUserIdAsync(Guid.Parse(userIdString));
                return Result<List<ChildDto>>.Success(_mapper.Map<List<ChildDto>>(children));
            }
            catch (Exception ex) { return Result<List<ChildDto>>.Failure(ex.Message); }
        }

        public async Task<Result<List<VaccineScheduleDto>>> GetVaccinesByChildIdAsync(Guid childId)
        {
            try
            {
                // dbo.VaccineSchedules ve dbo.Vaccines verilerini birlikte getiren metot
                var child = await _childRepository.GetWithVaccineScheduleAsync(childId);
                if (child == null) return Result<List<VaccineScheduleDto>>.Failure("Çocuk bulunamadı.");

                var dtos = child.VaccineSchedules
                    .Select(s => _mapper.Map<VaccineScheduleDto>(s))
                    .OrderBy(x => x.MonthIndex).ToList();

                return Result<List<VaccineScheduleDto>>.Success(dtos);
            }
            catch (Exception ex) { return Result<List<VaccineScheduleDto>>.Failure(ex.Message); }
        }

        private List<VaccineScheduleDto> ProcessSchedules(List<VaccineSchedule> schedules)
        {
            return schedules.Select(s => _mapper.Map<VaccineScheduleDto>(s))
                            .OrderBy(x => x.MonthIndex).ToList();
        }

        public async Task<Result<bool>> AddVaccineToChildAsync(Guid childId, string name, DateTime administeredDate)
        {
            try
            {
                var newSchedule = new VaccineSchedule { Id = Guid.NewGuid(), ChildId = childId, Period = "Manuel Giriş", ScheduledDate = administeredDate, CreatedAt = DateTime.Now };
                await _unitOfWork.GetRepository<VaccineSchedule>().AddAsync(newSchedule);

                bool isCompleted = administeredDate <= DateTime.Now;
                var newVaccine = new Vaccine { Id = Guid.NewGuid(), VaccineScheduleId = newSchedule.Id, ChildId = childId, Name = name, Date = administeredDate, Dose = "1. Doz", Status = isCompleted ? VaccineStatus.Completed : VaccineStatus.Pending, CompletedDate = isCompleted ? administeredDate : null };

                await _unitOfWork.GetRepository<Vaccine>().AddAsync(newVaccine);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex) { return Result<bool>.Failure(ex.Message); }
        }

        public Result<List<VaccineScheduleDto>> GetStandardSchedule(DateTime? birthDate = null)
        {
            try
            {
                var referenceDate = birthDate ?? DateTime.Now;
                var schedules = _scheduleGenerator.Generate(Guid.Empty, referenceDate);

                var dtos = schedules.Select(s => new VaccineScheduleDto(
                    s.Id,
                    s.Period,
                    s.MonthIndex,
                    s.ScheduledDate,
                    s.Vaccines.Select(v => new VaccineDto(v.Id, v.Name, v.Date, v.Dose, "pending", "Rehber", null, null, null)).ToList(),
                    false,
                    0,
                    s.Vaccines.Count)).ToList();

                return Result<List<VaccineScheduleDto>>.Success(dtos);
            }
            catch (Exception ex) { return Result<List<VaccineScheduleDto>>.Failure(ex.Message); }
        }
    }
}