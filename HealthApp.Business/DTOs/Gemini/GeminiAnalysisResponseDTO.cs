namespace HealthApp.Business.DTOs.Gemini
{
    public record GeminiAnalysisResponseDTO(
        List<ExtractedMedicineDTO> Medicines,
        List<ExtractedReminderDTO> Reminders
    );

    public record ExtractedMedicineDTO(
        string Name,
        string FrequencyType,
        int TimesPerDay,
        string? UsageInstructions
    );

    public record ExtractedReminderDTO(
        string Title,
        string Type,
        DateTime DateTime
    );
}
