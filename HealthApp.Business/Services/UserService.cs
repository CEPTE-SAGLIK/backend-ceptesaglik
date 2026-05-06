using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Business.DTOs;
using BCryptNet = BCrypt.Net.BCrypt;

namespace HealthApp.Business.Services
{
    public class UserService
    {
        // DataAccess katmanındaki Repository'i içeri alıyoruz. (Dependency Injection)
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Örnek 1: Dışarıdan DTO alıp, veritabanına Entity olarak kayıt etme metodu
        public async Task<UserDto> CreateUserAsync(UserCreateDTO createDto)
        {
            // 1. İş Mantığı / Doğrulama (Örneğin E-posta daha önce kullanılmış mı?)
            bool isEmailExists = await _userRepository.EmailExistsAsync(createDto.Email);
            if (isEmailExists)
            {
                throw new Exception("Bu e-posta adresi zaten kullanımda!");
            }

            // 2. DTO'yu veritabanının anladığı tipe (Entity'ye) ÇEVİRİYORUZ (Manuel Mapping)
            var newUser = new User
            {
                Name = createDto.Name,
                Surname = createDto.Surname,
                Email = createDto.Email,
                Password = BCryptNet.HashPassword(createDto.Password)
            };

            // 3. Veritabanına kaydetmesi için Repository'i çağırıyoruz
            var createdUser = await _userRepository.AddAsync(newUser);

            // 4. Controller'a Entity YERİNE sadece ihtiyaç olan bilgileri (UserDto) geriye dönüyoruz
            return new UserDto(createdUser.Id, createdUser.Name, createdUser.Surname, createdUser.Email, createdUser.CreatedAt);
        }

        // Örnek 2: Veritabanındaki Entity'leri çekip, Dış dünyadaki DTO'ya dönüştürüp listeleyen metot
        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var usersFromDb = await _userRepository.GetAllAsync();

            var userDtos = new List<UserDto>();
            foreach (var dbUser in usersFromDb)
            {
                userDtos.Add(new UserDto(dbUser.Id, dbUser.Name, dbUser.Surname, dbUser.Email, dbUser.CreatedAt));
            }

            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            return new UserDto(user.Id, user.Name, user.Surname, user.Email, user.CreatedAt);
        }

        public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return new UserDto(user.Id, user.Name, user.Surname, user.Email, user.CreatedAt);
        }
    }
}