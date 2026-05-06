using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    // Access token süresi dolduğunda, yeni token almak için gönderilen DTO.
    // Flutter eski refresh token'ı gönderir, backend yeni access + refresh token döner.
    public record RefreshTokenRequestDTO(
        string RefreshToken
    );
}
