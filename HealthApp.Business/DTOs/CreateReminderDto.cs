namespace HealthApp.Business.DTOs
{
    public class CreateReminderDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ReminderDate { get; set; }
        public int Type { get; set; } = 0;
        public int RepeatType { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string? RelatedItemId { get; set; }
        public string? MedicineId { get; set; }
        public string? VaccineId { get; set; }
    }
}
