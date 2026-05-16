namespace HealthApp.Business.DTOs
{
    public record PersonDto(
        Guid Id,
        Guid UserId,
        string Name,
        string Surname,
        DateTime BirthDate,
        string Gender,
        double? Height,
        double? Weight,
        List<string> ChronicDiseases,
        List<string> Allergies,
        DateTime CreatedAt,
        bool IsAccountOwner
    );
}
