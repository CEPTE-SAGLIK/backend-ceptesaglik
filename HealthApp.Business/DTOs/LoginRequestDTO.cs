using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    // Auth işlemleri (Login) için kullanılacak DTO
    // Sadece email ve password yeterli — kullanıcı zaten kayıtlı.
    public record LoginRequestDTO(
        string Email,
        string Password
    );
}
