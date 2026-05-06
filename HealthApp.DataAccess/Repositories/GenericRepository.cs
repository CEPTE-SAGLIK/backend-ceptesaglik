using HealthApp.DataAccess.Context;
using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        // 1. DİKKAT: 'protected' yapıyoruz ki, miras alan sınıflar (örn: UserRepository) bunlara erişebilsin.
        protected readonly AppDbContext _appDbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext appDbContext)
        { 
            _appDbContext = appDbContext;
            _dbSet = _appDbContext.Set<T>();
        }
        public async Task<List<T>> GetAllAsync()
        {
            // Veritabanındaki o tabloya (dbSet) git, hepsini Liste olarak (ToListAsync) getir.
            return await _dbSet.ToListAsync();
        }
        public async Task<T?> GetByIdAsync (Guid id)
        {
            // Veritabanındaki o tabloya (dbSet) git, id'ye göre tek bir kayıt getir.
            return await _dbSet.FindAsync(id);
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); // Soru işaretini kaldırdık
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _appDbContext.SaveChangesAsync();
        }

        // Entity nesnesi alarak silme metodu
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _appDbContext.SaveChangesAsync();
        }

        // Alternatif: Direkt ID ile silmek istersen diye yazılmış overload metot
        public async Task DeleteByIdAsync(Guid id)
        {
            var entity = await GetByIdAsync(id); // Dışarıdan gelen Id'ye göre veriyi bul.
            if (entity != null)
            {
                _dbSet.Remove(entity); // Bulduğun o veriyi sil.
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
