using HealthApp.Business.DTOs;
using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;

namespace HealthApp.Business.Services
{
    public class PersonService
    {
        private readonly PersonRepository _personRepository;
        private readonly AllergyRepository _allergyRepository;
        private readonly IllnessRepository _illnessRepository;
        private readonly VaccineRepository _vaccineRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly IReminderRepository _reminderRepository;
        private readonly MedicineRepository _medicineRepository;

        public PersonService(
            PersonRepository personRepository,
            AllergyRepository allergyRepository,
            IllnessRepository illnessRepository,
            VaccineRepository vaccineRepository,
            NotificationRepository notificationRepository,
            IReminderRepository reminderRepository,
            MedicineRepository medicineRepository)
        {
            _personRepository = personRepository;
            _allergyRepository = allergyRepository;
            _illnessRepository = illnessRepository;
            _vaccineRepository = vaccineRepository;
            _notificationRepository = notificationRepository;
            _reminderRepository = reminderRepository;
            _medicineRepository = medicineRepository;
        }

        public async Task<PersonDto> CreatePersonAsync(PersonCreateDTO dto, Guid userId, bool isAccountOwner = false)
        {
            if (!Enum.TryParse<Gender>(dto.Gender, true, out var gender))
            {
                throw new Exception("Geçersiz cinsiyet değeri. male/female gönderin.");
            }

            // Her hesapta en fazla bir kayıt sahibi profili olabilir.
            var owner = isAccountOwner;
            if (owner)
            {
                var existing = await _personRepository.GetByUserIdAsync(userId);
                if (existing.Any(p => p.IsAccountOwner))
                {
                    owner = false;
                }
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
                Allergies = dto.Allergies ?? new List<string>(),
                IsAccountOwner = owner
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

        public async Task DeletePersonAsync(Guid personId, Guid userId)
        {
            var person = await _personRepository.GetByIdAndUserIdAsync(personId, userId);
            if (person == null)
            {
                throw new Exception("Kayıt bulunamadı veya bu kullanıcıya ait değil.");
            }
            if (person.IsAccountOwner)
            {
                throw new Exception("Hesap sahibinin profili silinemez.");
            }

            // İlişkili kayıtları FK sırasına göre temizle.

            // Direkt PersonId ile bağlı hatırlatıcıları sil.
            var directReminders = await _reminderRepository.GetByPersonIdAsync(personId);
            foreach (var r in directReminders) await _reminderRepository.DeleteAsync(r);

            // İlaçları ve onlara bağlı hatırlatıcıları sil.
            var medicines = await _medicineRepository.GetByPersonIdAsync(personId);
            foreach (var m in medicines)
            {
                var medReminders = (await _reminderRepository.GetAllAsync())
                    .Where(r => r.MedicineId == m.Id).ToList();
                foreach (var r in medReminders) await _reminderRepository.DeleteAsync(r);
                await _medicineRepository.DeleteAsync(m);
            }

            var allergies = await _allergyRepository.GetByPersonIdAsync(personId);
            foreach (var a in allergies) await _allergyRepository.DeleteAsync(a);

            var illnesses = await _illnessRepository.GetByPersonIdAsync(personId);
            foreach (var i in illnesses) await _illnessRepository.DeleteAsync(i);

            var notifications = await _notificationRepository.GetByPersonIdAsync(personId);
            foreach (var n in notifications) await _notificationRepository.DeleteAsync(n);

            // Aşılara bağlı hatırlatıcıları önce sil, sonra aşıları sil.
            var vaccines = await _vaccineRepository.GetByPersonIdAsync(personId);
            foreach (var v in vaccines)
            {
                var reminders = await _reminderRepository.GetByVaccineIdAsync(v.Id);
                foreach (var r in reminders) await _reminderRepository.DeleteAsync(r);
                await _vaccineRepository.DeleteAsync(v);
            }

            await _personRepository.DeleteAsync(person);
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
                person.CreatedAt,
                person.IsAccountOwner
            );
        }
    }
}
