using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace HealthApp.Business.DTOs
{
    public record VaccineScheduleDto(
        [property: JsonPropertyName("id")] Guid Id,
        [property: JsonPropertyName("period")] string Period,
        [property: JsonPropertyName("monthIndex")] int MonthIndex,
        [property: JsonPropertyName("scheduledDate")] DateTime ScheduledDate,
        [property: JsonPropertyName("vaccines")] List<VaccineDto> Vaccines,
        [property: JsonPropertyName("isCompleted")] bool IsCompleted,
        [property: JsonPropertyName("completedCount")] int CompletedCount,
        [property: JsonPropertyName("pendingCount")] int PendingCount
    );
}
