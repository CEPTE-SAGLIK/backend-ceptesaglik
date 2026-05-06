using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Domain.Entities
{
   public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Refresh Token: Uzun ömürlü yenileme anahtarı.
        // Access token süresi dolduğunda, bu token ile yeni access token alınır.
        // Rastgele üretilen bir string — JWT gibi yapılandırılmış değil, sadece benzersiz bir anahtar.
        public string? RefreshToken { get; set; }

        // Refresh Token'ın son kullanma tarihi.
        // Süresi dolmuşsa kullanıcı tekrar login olmak zorunda.
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
