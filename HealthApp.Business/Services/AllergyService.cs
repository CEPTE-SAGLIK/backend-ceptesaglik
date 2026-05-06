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
                // Eğer Flutter'dan tarih gelmezse, şu anın tarihini kaydet dedik
                CreatedDate = createDto.CreatedDate != default ? createDto.CreatedDate : DateTime.Now
            };

            await _repository.AddAsync(allergy);

            return new AllergyDto
            {
                Id = allergy.Id,
                PersonId = allergy.PersonId,
                Name = allergy.Name,
                CreatedDate = allergy.CreatedDate
            };
        }

        // 2. Tüm Alerjileri Getirme
        public async Task<List<AllergyDto>> GetAllAllergiesAsync()
        {
            var allergies = await _repository.GetAllAsync();

            return allergies.Select(a => new AllergyDto
            {
                Id = a.Id,
                PersonId = a.PersonId,
                Name = a.Name,
                CreatedDate = a.CreatedDate
            }).ToList();
        }

        // 3. İstenen Kişiye Ait (PersonId) Alerjileri Getirme -> Veri İzolasyonu (GÜVENLİK)
        public async Task<List<AllergyDto>> GetAllByPersonIdAsync(Guid personId)
        {
            var allergies = await _repository.GetByPersonIdAsync(personId);

            return allergies.Select(a => new AllergyDto
            {
                Id = a.Id,
                PersonId = a.PersonId,
                Name = a.Name,
                CreatedDate = a.CreatedDate
            }).ToList();
        }

        // 4. Alerji Silme Metodu
        public async Task<bool> DeleteAllergyAsync(Guid id)
        {
            var allergy = await _repository.GetByIdAsync(id);
            if (allergy == null) return false;

            await _repository.DeleteAsync(allergy);
            return true;
        }
    }
}