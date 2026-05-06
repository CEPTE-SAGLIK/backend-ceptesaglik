using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Repositories;
using HealthApp.Business.DTOs;

namespace HealthApp.Business.Services
{
    public class IllnessService
    {
        private readonly IllnessRepository _repository;

        public IllnessService(IllnessRepository repository)
        {
            _repository = repository;
        }

        // 1. Yeni Hastalık Ekleme Metodu
        public async Task<IllnessDto> AddIllnessAsync(IllnessCreateDto createDto)
        {
            var illness = new Illness
            {
                PersonId = createDto.PersonId,
                Name = createDto.Name,
                DiagnosisDate = createDto.DiagnosisDate,
                Status = string.IsNullOrEmpty(createDto.Status) ? "active" : createDto.Status,
                DoctorNotes = createDto.DoctorNotes
            };

            await _repository.AddAsync(illness);

            return new IllnessDto
            {
                Id = illness.Id,
                PersonId = illness.PersonId,
                Name = illness.Name,
                DiagnosisDate = illness.DiagnosisDate,
                Status = illness.Status,
                DoctorNotes = illness.DoctorNotes
            };
        }

        // 2. Tüm Hastalıkları Getirme Metodu
        public async Task<List<IllnessDto>> GetAllIllnessesAsync()
        {
            var illnesses = await _repository.GetAllAsync();

            return illnesses.Select(i => new IllnessDto
            {
                Id = i.Id,
                PersonId = i.PersonId,
                Name = i.Name,
                DiagnosisDate = i.DiagnosisDate,
                Status = i.Status,
                DoctorNotes = i.DoctorNotes
            }).ToList();
        }

        // 3. İstenen Kişiye Ait (PersonId) Hastalıkları Getirme -> Veri İzolasyonu (GÜVENLİK)
        public async Task<List<IllnessDto>> GetAllByPersonIdAsync(Guid personId)
        {
            var illnesses = await _repository.GetByPersonIdAsync(personId);

            return illnesses.Select(i => new IllnessDto
            {
                Id = i.Id,
                PersonId = i.PersonId,
                Name = i.Name,
                DiagnosisDate = i.DiagnosisDate,
                Status = i.Status,
                DoctorNotes = i.DoctorNotes
            }).ToList();
        }

        // IllnessService.cs içine ekle:
        public async Task<bool> DeleteIllnessAsync(Guid id)
        {
            var illness = await _repository.GetByIdAsync(id); // Önce hastalığı bulalım
            if (illness == null) return false;

            await _repository.DeleteAsync(illness); // Tablodan uçuralım
            return true;
        }
    }
}