using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    // Auth işlemleri (Register) için kullanılacak DTO
    public record RegisterRequestDTO(
        string Name,
        string Surname,
        string Email,
        string Password,
        string ConfirmPassword
    );
}
