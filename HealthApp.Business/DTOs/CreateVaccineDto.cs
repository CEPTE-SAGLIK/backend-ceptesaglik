using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HealthApp.Business.DTOs
{
    public class CreateVaccineDto
    {
        public string Name { get; set; } = string.Empty;

        // Eğer çocuk ise bu alan dolacak
        public string? ChildId { get; set; }

        // Eğer yetişkin (aile üyesi/profil) ise bu alan dolacak
        public string? PersonId { get; set; }

        public string Date { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Status { get; set; }

        public string? Dose { get; set; }

        // Kullanıcının seçtiği sıklık bilgisi
        public string? Frequency { get; set; }
    }
}
