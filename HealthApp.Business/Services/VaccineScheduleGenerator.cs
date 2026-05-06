using System;
using System.Collections.Generic;
using System.Linq;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;

namespace HealthApp.Business.Services
{
    public class VaccineScheduleGenerator
    {
        public List<VaccineSchedule> Generate(Guid childId, DateTime birthDate)
        {
            var template = new[]
            {
                new { Month = 0,  Period = "Doğumda",             Vaccines = new[] { ("Hepatit B", "1. Doz") } },
                new { Month = 1,  Period = "1. Ay Sonu",          Vaccines = new[] { ("Hepatit B", "2. Doz") } },
                new { Month = 2,  Period = "2. Ay Sonu",          Vaccines = new[] { ("BCG (Verem)", "1. Doz"), ("5'li Karma", "1. Doz"), ("KPA (Pnömokok)", "1. Doz") } },
                new { Month = 4,  Period = "4. Ay Sonu",          Vaccines = new[] { ("5'li Karma", "2. Doz"), ("KPA (Pnömokok)", "2. Doz") } },
                new { Month = 6,  Period = "6. Ay Sonu",          Vaccines = new[] { ("Hepatit B", "3. Doz"), ("5'li Karma", "3. Doz"), ("OPA (Çocuk Felci)", "1. Doz") } },
                new { Month = 12, Period = "12. Ay (1 Yaş)",      Vaccines = new[] { ("KKK", "1. Doz"), ("KPA (Pnömokok)", "Pekiştirme"), ("Su Çiçeği", "1. Doz") } },
                new { Month = 18, Period = "18. Ay",              Vaccines = new[] { ("4'lü Karma", "Pekiştirme"), ("OPA (Çocuk Felci)", "2. Doz"), ("Hepatit A", "1. Doz") } },
                new { Month = 24, Period = "24. Ay (2 Yaş)",      Vaccines = new[] { ("Hepatit A", "2. Doz") } },
                // EKSİK OLANLAR EKLENDİ:
                new { Month = 48, Period = "48. Ay (4 Yaş)",      Vaccines = new[] { ("4'lü Karma", "Rapel"), ("KKK", "2. Doz") } },
                new { Month = 72, Period = "1. Sınıf (6 Yaş)",   Vaccines = new[] { ("Td (Difteri-Tetanoz)", "Rapel"), ("OPA (Çocuk Felci)", "Rapel") } },
            };

            return template.Select(t =>
            {
                var scheduledDate = birthDate.AddMonths(t.Month);
                return new VaccineSchedule
                {
                    Id = Guid.NewGuid(),
                    ChildId = childId,
                    Period = t.Period,
                    MonthIndex = t.Month,
                    ScheduledDate = scheduledDate,
                    Vaccines = t.Vaccines.Select(v => new Vaccine
                    {
                        Id = Guid.NewGuid(),
                        Name = v.Item1,
                        Date = scheduledDate,
                        Dose = v.Item2,
                        Status = VaccineStatus.Pending,
                        ChildId = childId
                    }).ToList()
                };
            }).ToList();
        }
    }
}