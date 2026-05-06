using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HealthApp.Domain.Enums;

namespace HealthApp.Business.DTOs
{
    public class ChildDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public double? Height { get; set; } // Controller'daki hatayı çözer
        public double? Weight { get; set; } // Controller'daki hatayı çözer
        public List<VaccineDto> Vaccines { get; set; } = new(); // Service'teki hatayı çözer
        public List<VaccineScheduleDto> VaccineSchedule { get; set; } = new();
    }
}