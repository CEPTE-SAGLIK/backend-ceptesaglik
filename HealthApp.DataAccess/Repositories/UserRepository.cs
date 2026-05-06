using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.DataAccess.Repositories
{
    // 1. DİKKAT: <T> değil, doğrudan <User> veriyoruz.
    // 2. Artık bu depo sadece User Entity'si için çalışacak.
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        // Genel (Generic) repoda "GetByEmail" olamaz çünkü her tablonun E-maili yoktur.
        // Ama User'ın vardır. O zaman User'a özel metodu buraya yazıyoruz!
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Email daha önce kullanılmış mı diye kontrol eden yardımcı metot
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        // Refresh token ile kullanıcıyı bul.
        // Access token süresi dolduğunda, Flutter refresh token gönderir.
        // Bu metot o refresh token'a sahip kullanıcıyı bulur.
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}
