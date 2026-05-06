using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using HealthApp.Business.DTOs;
using BCryptNet = BCrypt.Net.BCrypt;

namespace HealthApp.Business.Services
{
    public class AuthService
    {
        // Aynı UserRepository'yi kullanıyoruz — yeni bir repo yazmaya gerek yok.
        // Çünkü Auth işlemleri de sonuçta User tablosuna gidiyor.
        private readonly UserRepository _userRepository;
        // Token üretimi için JwtService enjekte ediliyor.
        private readonly JwtService _jwtService;

        public AuthService(UserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Kullanıcı kayıt işlemi.
        /// Flutter'dan RegisterRequestDTO gelir, doğrulanır, DB'ye kaydedilir, UserDto döner.
        /// </summary>
        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO registerDto)
        {
            // 1. Şifre eşleşme kontrolü (ConfirmPassword ile)
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new Exception("Şifreler eşleşmiyor!");
            }

            // 2. E-posta daha önce kullanılmış mı?
            bool emailExists = await _userRepository.EmailExistsAsync(registerDto.Email);
            if (emailExists)
            {
                throw new Exception("Bu e-posta adresi zaten kullanımda!");
            }

            // 3. DTO → Entity dönüşümü (Manuel Mapping)
            var newUser = new User
            {
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                Email = registerDto.Email,
                Password = BCryptNet.HashPassword(registerDto.Password)
            };

            // 4. Veritabanına kaydet
            var createdUser = await _userRepository.AddAsync(newUser);

            // 5. Access token + Refresh token üret
            var accessToken = _jwtService.GenerateToken(createdUser);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // 6. Refresh token'ı veritabanına kaydet
            // Neden DB'ye kaydediyoruz? Çünkü refresh token geldiğinde
            // "bu token gerçekten bu kullanıcıya ait mi?" diye kontrol edeceğiz.
            createdUser.RefreshToken = refreshToken;
            createdUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpireDays());
            await _userRepository.UpdateAsync(createdUser);

            // 7. Token + Kullanıcı bilgilerini birlikte döndür
            var userDto = new UserDto(
                createdUser.Id,
                createdUser.Name,
                createdUser.Surname,
                createdUser.Email,
                createdUser.CreatedAt
            );

            return new AuthResponseDTO(accessToken, refreshToken, userDto);
        }

        /// <summary>
        /// Kullanıcı giriş işlemi.
        /// Flutter'dan LoginRequestDTO gelir, e-posta ve şifre kontrol edilir, UserDto döner.
        /// </summary>
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO loginDto)
        {
            // 1. E-posta ile kullanıcıyı bul
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new Exception("E-posta veya şifre hatalı!");
            }

            // 2. BCrypt.Verify ile şifre doğrulama
            if (!BCryptNet.Verify(loginDto.Password, user.Password))
            {
                throw new Exception("E-posta veya şifre hatalı!");
            }

            // 3. Access token + Refresh token üret
            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // 4. Refresh token'ı veritabanına kaydet
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpireDays());
            await _userRepository.UpdateAsync(user);

            // 5. Token + Kullanıcı bilgilerini birlikte döndür
            var userDto = new UserDto(
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.CreatedAt
            );

            return new AuthResponseDTO(accessToken, refreshToken, userDto);
        }

        /// <summary>
        /// Refresh Token ile yeni Access Token alma.
        /// Flutter access token süresi dolduğunda bu metodu çağırır.
        /// Eski refresh token verilir → Yeni access token + yeni refresh token döner.
        /// </summary>
        public async Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO refreshDto)
        {
            // 1. Refresh token ile kullanıcıyı bul
            var user = await _userRepository.GetByRefreshTokenAsync(refreshDto.RefreshToken);
            if (user == null)
            {
                throw new Exception("Geçersiz refresh token!");
            }

            // 2. Refresh token'ın süresi dolmuş mu?
            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                // Süresi dolmuş → Eski token'ı sil, kullanıcı tekrar login olmalı
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userRepository.UpdateAsync(user);
                throw new Exception("Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.");
            }

            // 3. Yeni access token + yeni refresh token üret (Token Rotation)
            // Neden yeni refresh token? Eski token çalınmış olabilir.
            // Her kullanımda yenisi üretilir → çalınan eski token geçersiz olur.
            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // 4. Yeni refresh token'ı DB'ye kaydet
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpireDays());
            await _userRepository.UpdateAsync(user);

            // 5. Yeni token'ları döndür
            var userDto = new UserDto(
                user.Id,
                user.Name,
                user.Surname,
                user.Email,
                user.CreatedAt
            );

            return new AuthResponseDTO(newAccessToken, newRefreshToken, userDto);
        }
    }
}
