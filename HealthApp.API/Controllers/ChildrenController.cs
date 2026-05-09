using HealthApp.Business.DTOs;
using HealthApp.Business.Services;
using HealthApp.Business.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HealthApp.API.Controllers
{
    public class AddVaccineRequest
    {
        public string VaccineName { get; set; } = string.Empty;
        public DateTime DateAdministered { get; set; }
    }

    public class AddVaccineToScheduleRequest
    {
        public string VaccineName { get; set; } = string.Empty;
        public string Dose { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public DateTime Date { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChildrenController : ControllerBase
    {
        private readonly ChildService _childService;
        public ChildrenController(ChildService childService) { _childService = childService; }

        [HttpGet("standard-schedule")]
        [AllowAnonymous]
        // DÜZELTİLDİ: Opsiyonel birthDate parametresi eklendi
        public IActionResult GetStandardSchedule([FromQuery] DateTime? birthDate)
        {
            var result = _childService.GetStandardSchedule(birthDate);
            return result.IsSuccess
                ? Ok(ApiResponse<List<VaccineScheduleDto>>.Ok(result.Data!))
                : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpGet("{id}/vaccines")]
        public async Task<IActionResult> GetVaccines(Guid id)
        {
            var result = await _childService.GetVaccinesByChildIdAsync(id);
            return result.IsSuccess
                ? Ok(ApiResponse<List<VaccineScheduleDto>>.Ok(result.Data!))
                : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpPost("{id}/vaccines")]
        public async Task<IActionResult> AddVaccine(Guid id, [FromBody] AddVaccineRequest request)
        {
            var result = await _childService.AddVaccineToChildAsync(id, request.VaccineName, request.DateAdministered);
            return result.IsSuccess
                ? Ok(ApiResponse<bool>.Ok(true, "Aşı başarıyla eklendi."))
                : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpPost("{childId}/schedules/{scheduleId}/vaccines")]
        public async Task<IActionResult> AddVaccineToSchedule(Guid childId, Guid scheduleId, [FromBody] AddVaccineToScheduleRequest request)
        {
            var result = await _childService.AddVaccineToScheduleAsync(childId, scheduleId, request.VaccineName, request.Dose, request.Status, request.Date);
            return result.IsSuccess
                ? Ok(ApiResponse<bool>.Ok(true, "Aşı takvime eklendi."))
                : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChildDto createDto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized(ApiResponse<object>.Fail("Token geçersiz."));
                var userId = Guid.Parse(userIdString);
                var result = await _childService.CreateAsync(userId, createDto);
                return result.IsSuccess ? Ok(ApiResponse<ChildDto>.Ok(result.Data!)) : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
            }
            catch (Exception ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        }

        [HttpDelete("{id}/schedules/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(Guid id, Guid scheduleId)
        {
            var result = await _childService.DeleteScheduleAsync(id, scheduleId);
            return result.IsSuccess ? Ok(ApiResponse<bool>.Ok(true, "Takvim silindi.")) : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _childService.DeleteAsync(id);
            return result.IsSuccess ? Ok(ApiResponse<bool>.Ok(true, "Kayıt silindi.")) : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _childService.GetAllAsync();
            return result.IsSuccess ? Ok(ApiResponse<List<ChildDto>>.Ok(result.Data!)) : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }

        [HttpPut("{id}/physical-info")]
        public async Task<IActionResult> UpdatePhysical(Guid id, [FromBody] ChildDto updateDto)
        {
            var result = await _childService.UpdatePhysicalInfoAsync(id, updateDto.Height ?? 0, updateDto.Weight ?? 0, updateDto.Gender);
            return result.IsSuccess ? Ok(ApiResponse<ChildDto>.Ok(result.Data!)) : BadRequest(ApiResponse<object>.Fail(result.ErrorMessage!));
        }
    }
}