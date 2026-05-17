using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Domain.Entities;
using HealthApp.DataAccess.Repositories;
using HealthApp.Business.DTOs;

namespace HealthApp.Business.Services
{
    public class AllergyService
    {
        private readonly AllergyRepository _repository;

        public AllergyService(AllergyRepository repository)
        {
            _repository = repository;
        }

        // 1. Yeni Alerji Ekleme
        public async Task<AllergyDto> AddAllergyAsync(AllergyCreateDto createDto)
        {
            var allergy = new Allergy
            {
                PersonId = createDto.PersonId,
                Name = createDto.Name,
                Level = createDto.Level,
                Description = createDto.Description,
                CreatedDate = createDto.CreatedDate != default ? createDto.CreatedDate : DateTime.Now
            };

            await _repository.AddAsync(allergy);
            return ToDto(allergy);
        }

        // 2. Tüm Alerjileri Getirme
        public async Task<List<AllergyDto>> GetAllAllergiesAsync()
        {
            var allergies = await _repository.GetAllAsync();
            return allergies.Select(ToDto).ToList();
        }

        // 3. İstenen Kişiye Ait (PersonId) Alerjileri Getirme -> Veri İzolasyonu (GÜVENLİK)
        public async Task<List<AllergyDto>> GetAllByPersonIdAsync(Guid personId)
        {
            var allergies = await _repository.GetByPersonIdAsync(personId);
            return allergies.Select(ToDto).ToList();
        }

        // 4. Alerji Güncelleme
        public async Task<AllergyDto?> UpdateAllergyAsync(Guid id, AllergyUpdateDto updateDto)
        {
            var allergy = await _repository.GetByIdAsync(id);
            if (allergy == null) return null;

            allergy.Name = updateDto.Name;
            allergy.Level = updateDto.Level;
            allergy.Description = updateDto.Description;

            await _repository.UpdateAsync(allergy);
            return ToDto(allergy);
        }

        // 5. Alerji Silme Metodu
        public async Task<bool> DeleteAllergyAsync(Guid id)
        {
            var allergy = await _repository.GetByIdAsync(id);
            if (allergy == null) return false;

            await _repository.DeleteAsync(allergy);
            return true;
        }

        private static AllergyDto ToDto(Allergy a) => new AllergyDto
        {
            Id = a.Id,
            PersonId = a.PersonId,
            Name = a.Name,
            Level = a.Level,
            Description = a.Description,
            CreatedDate = a.CreatedDate
        };
    }
}