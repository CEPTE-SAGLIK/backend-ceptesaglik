namespace HealthApp.Business.DTOs
{
    public record PersonUpdateDTO(
        string? Name,
        string? Surname,
        DateTime? BirthDate,
        string? Gender,
        double? Height,
        double? Weight,
        List<string>? ChronicDiseases,
        List<string>? Allergies
    );
}
