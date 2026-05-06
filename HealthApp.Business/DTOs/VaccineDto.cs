using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HealthApp.Business.DTOs
{
    public record VaccineDto(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("date")] DateTime Date,
        [property: JsonPropertyName("dose")] string Dose,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("nextDoseDate")] DateTime? NextDoseDate,
        [property: JsonPropertyName("completedDate")] DateTime? CompletedDate,
        [property: JsonPropertyName("childId")] Guid? ChildId
    );
}
