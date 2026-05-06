using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.Domain.Enums; // Enum'ları kullanabilmek için ekledik

namespace HealthApp.Domain.Entities
{
    public class Child : BaseEntity
    {
        // 1. Çocuğun hangi kullanıcıya ait olduğunu tutan özellik (Veritabanında UserId sütunu olacak)
        public Guid UserId { get; set; }

        // Mevcut özelliğin (Zaten eklemiştin)
        public string Name { get; set; } = string.Empty;

        // 2. Çocuğun Doğum Tarihi (Aşı takvimi hesaplamak için çok önemli)
        public DateTime BirthDate { get; set; }

        // 3. Çocuğun Cinsiyeti (Az önce oluşturduğumuz Enum'ı kullanıyoruz)
        public Gender Gender { get; set; }

        public double Height { get; set; }
        public double Weight { get; set; }

        // --- İLİŞKİLER (Navigation Properties) ---
        // Bu çocuğu, veritabanındaki User (Kullanıcı) tablosundaki kişiye bağlayan "köprü"
        public User User { get; set; } = null!;

        public ICollection<Vaccine> Vaccines { get; set; } = new List<Vaccine>();
        public ICollection<VaccineSchedule> VaccineSchedules { get; set; } = new List<VaccineSchedule>();
    }
}
