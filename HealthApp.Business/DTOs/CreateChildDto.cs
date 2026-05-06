using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthApp.Domain.Enums;

namespace HealthApp.Business.DTOs
{
    public class CreateChildDto
    {
        // Frontend 'Name' gönderiyor
        public string Name { get; set; } = string.Empty;

        // Frontend 'BirthDate' gönderiyor
        public DateTime BirthDate { get; set; }

        // DÜZELTİLDİ: Enum eşleşme hatasını önlemek için 'int' yapıldı
        public int Gender { get; set; }

        // Frontend 'UserId' gönderiyor
        public Guid UserId { get; set; }
    }
}
