using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    // Dışarıdan veri alırken kullanacağımız DTO (Id yok, Password var)
    public record UserCreateDTO(
        string Name,
        string Surname,
        string Email,
        string Password
    );
}
