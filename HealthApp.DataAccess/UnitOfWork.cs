using HealthApp.DataAccess.Context;
using HealthApp.DataAccess.Repositories;
using HealthApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthApp.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // DÜZELTME: Dönüş tipi interface ile birebir aynı (IMedicineRepository) olmalı
        public IMedicineRepository Medicines => (IMedicineRepository)GetRepository<Medicine>();

        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IGenericRepository<T>)_repositories[typeof(T)];
            }

            object repository;

            // Medicine için özel repository oluşturuyoruz ki IMedicineRepository interface'ini karşılasın
            if (typeof(T) == typeof(Medicine))
            {
                repository = new MedicineRepository(_context);
            }
            else
            {
                repository = new GenericRepository<T>(_context);
            }

            _repositories.Add(typeof(T), repository);
            return (IGenericRepository<T>)repository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}