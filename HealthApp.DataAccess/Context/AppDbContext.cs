using HealthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthApp.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccineSchedule> VaccineSchedules { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Illness> Illnesses { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Mevcut Aşı Takvimi İlişkileri
            modelBuilder.Entity<VaccineSchedule>()
                .HasOne(vs => vs.Child)
                .WithMany(c => c.VaccineSchedules)
                .HasForeignKey(vs => vs.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vaccine>()
                .HasOne(v => v.VaccineSchedule)
                .WithMany(vs => vs.Vaccines)
                .HasForeignKey(v => v.VaccineScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Reminder - Medicine İlişkisi
            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.Medicine)
                .WithMany(m => m.Reminders)
                .HasForeignKey(r => r.MedicineId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Person - Vaccine İlişkisi (Yetişkin Aşıları - Çoklu Cascade Çözümü)
            // SQL Server'da çakışmayı (multiple cascade paths) önlemek amacıyla Cascade yerine NoAction kullanılmıştır.
            // Vaccines tablosunda VaccineSchedule üzerinden zaten etkin bir cascade yolu bulunmaktadır.
            modelBuilder.Entity<Vaccine>()
                .HasOne(v => v.Person)
                .WithMany(p => p.Vaccines)
                .HasForeignKey(v => v.PersonId)
                .OnDelete(DeleteBehavior.NoAction);

            // 4. Reminder - Vaccine İlişkisi
            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.Vaccine)
                .WithMany()
                .HasForeignKey(r => r.VaccineId)
                .OnDelete(DeleteBehavior.NoAction);

            // 5. Reminder - User İlişkisi
            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}