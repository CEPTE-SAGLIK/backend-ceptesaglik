using HealthApp.Business.DTOs;
using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;

namespace HealthApp.Business.Services
{
    public class PersonService
    {
        private readonly PersonRepository _personRepository;

        public PersonService(PersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<PersonDto> CreatePersonAsync(PersonCreateDTO dto, Guid userId)
        {
            if (!Enum.TryParse<Gender>(dto.Gender, true, out var gender))
            {
                throw new Exception("Geçersiz cinsiyet değeri. male/female gönderin.");
            }

            var person = new Person
            {
                UserId = userId,
                Name = dto.Name,
                Surname = dto.Surname,
                BirthDate = dto.BirthDate,
                Gender = gender,
                Height = dto.Height,
                Weight = dto.Weight,
                ChronicDiseases = dto.ChronicDiseases ?? new List<string>(),
                Allergies = dto.Allergies ?? new List<string>()
            };

            var created = await _personRepository.AddAsync(person);
            return MapToDto(created);
        }

        public async Task<List<PersonDto>> GetPersonsByUserIdAsync(Guid userId)
        {
            var persons = await _personRepository.GetByUserIdAsync(userId);
            return persons.Select(MapToDto).ToList();
        }

        public async Task<PersonDto> UpdatePersonAsync(Guid personId, PersonUpdateDTO dto, Guid userId)
        {
            var person = await _personRepository.GetByIdAndUserIdAsync(personId, userId);
            if (person == null)
            {
                throw new Exception("Kayıt bulunamadı veya bu kullanıcıya ait değil.");
            }

            if (dto.Name is not null) person.Name = dto.Name;
            if (dto.Surname is not null) person.Surname = dto.Surname;
            if (dto.BirthDate.HasValue) person.BirthDate = dto.BirthDate.Value;
            if (dto.Height.HasValue) person.Height = dto.Height;
            if (dto.Weight.HasValue) person.Weight = dto.Weight;
            if (dto.ChronicDiseases is not null) person.ChronicDiseases = dto.ChronicDiseases;
            if (dto.Allergies is not null) person.Allergies = dto.Allergies;

            if (dto.Gender is not null)
            {
                if (!Enum.TryParse<Gender>(dto.Gender, true, out var gender))
                {
                    throw new Exception("Geçersiz cinsiyet değeri. male/female gönderin.");
                }
                person.Gender = gender;
            }

            await _personRepository.UpdateAsync(person);
            return MapToDto(person);
        }

        public async Task<PersonDto> GetPersonByIdAsync(Guid personId, Guid userId)
        {
            var person = await _personRepository.GetByIdAndUserIdAsync(personId, userId);
            if (person == null)
            {
                throw new Exception("Kayıt bulunamadı veya bu kullanıcıya ait değil.");
            }

            return MapToDto(person);
        }

        private static PersonDto MapToDto(Person person)
        {
            return new PersonDto(
                person.Id,
                person.UserId,
                person.Name,
                person.Surname,
                person.BirthDate,
                person.Gender.ToString(),
                person.Height,
                person.Weight,
                person.ChronicDiseases,
                person.Allergies,
                person.CreatedAt
            );
        }
    }
}
