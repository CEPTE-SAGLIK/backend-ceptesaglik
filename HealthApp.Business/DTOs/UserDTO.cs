using System;

namespace HealthApp.Business.DTOs
{
    // C# 9.0 ile gelen "record" özelliği. DTO'lar için en iyi formattır!
    public record UserDto(
        Guid Id,
        string Name,
        string Surname,
        string Email,
        DateTime CreatedAt
    ); 
    // Şifre vb hiç bir şey beklemiyoruz!
}
