using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HealthApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthApp.Business.Services
{
    public class JwtService
    {
        // IConfiguration: appsettings.json'daki ayarları okumak için
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// User entity'sinden JWT token üretir.
        /// Token içine kullanıcı bilgileri (Claims) gömülür.
        /// Flutter bu token'ı her istekte "Authorization: Bearer ..." olarak gönderir.
        /// Backend token'ı doğrular ve Claims'ten kullanıcıyı tanır.
        /// </summary>
        public string GenerateToken(User user)
        {
            // 1. appsettings.json'dan JWT ayarlarını oku
            var key = _configuration["Jwt:Key"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);

            // 2. Claims: Token'ın içine gömülen kullanıcı bilgileri.
            // Backend sonraki isteklerde bu bilgileri token'dan çıkararak kullanıcıyı tanır.
            // Şifre gibi hassas bilgiler ASLA claim'e eklenmez!
            var claims = new[]
            {
                // Sub (Subject): Kullanıcının benzersiz kimliği
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // Kullanıcı adı
                new Claim("name", user.Name),
                // Jti: Her token'a benzersiz ID verir (token tekrar kullanımını engellemek için)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 3. Signing Key: appsettings'teki gizli anahtardan imzalama anahtarı oluştur
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // 4. Credentials: Hangi anahtar ve hangi algoritma ile imzalanacak
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 5. Token'ı oluştur
            var token = new JwtSecurityToken(
                issuer: issuer,       // Kim üretti
                audience: audience,   // Kimin için
                claims: claims,       // İçindeki bilgiler
                expires: DateTime.UtcNow.AddMinutes(expireMinutes), // Ne zaman sona erecek
                signingCredentials: credentials  // İmza
            );

            // 6. Token nesnesini string'e çevir ("eyJhbGciOiJI..." formatında)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Rastgele, benzersiz bir refresh token üretir.
        /// JWT'den farklı olarak yapılandırılmış değil — sadece kriptografik olarak güvenli bir string.
        /// Bu string veritabanında saklanır ve yeni access token almak için kullanılır.
        /// </summary>
        public string GenerateRefreshToken()
        {
            // 64 byte rastgele veri üret ve Base64'e çevir
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Refresh token'ın kaç gün geçerli olacağını appsettings'ten okur.
        /// </summary>
        public int GetRefreshTokenExpireDays()
        {
            return int.Parse(_configuration["Jwt:RefreshTokenExpireDays"]!);
        }
    }
}
