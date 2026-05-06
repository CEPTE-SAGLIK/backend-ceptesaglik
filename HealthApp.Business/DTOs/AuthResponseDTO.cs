using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    // Login ve Register sonrası döndürülecek DTO.
    // Flutter'da:
    //   _authToken = response['token'];           → Access Token (kısa ömürlü, her istekte gönderilir)
    //   _refreshToken = response['refreshToken']; → Refresh Token (uzun ömürlü, yeni access token almak için)
    //   _currentUser = User.fromJson(response['user']);
    public record AuthResponseDTO(
        string Token,
        string RefreshToken,
        UserDto User
    );
}
