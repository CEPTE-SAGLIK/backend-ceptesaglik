using System;

namespace HealthApp.Business.DTOs
{
    public class ReminderDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DateTime { get; set; } = string.Empty;
        // Lowercase string names matching Flutter's ReminderType / RepeatType enum names
        public string Type { get; set; } = "custom";
        public string RepeatType { get; set; } = "none";
        public bool IsActive { get; set; } = true;
        public bool IsCompleted { get; set; } = false;
        public string? RelatedItemId { get; set; }
        public string? MedicineId { get; set; }
        public string? VaccineId { get; set; }
    }
}
