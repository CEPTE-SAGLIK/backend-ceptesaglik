using HealthApp.Business.Services;
using HealthApp.Business.DTOs;
using HealthApp.Business.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthApp.DataAccess;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;

namespace HealthApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaccinesController : ControllerBase
    {
        private readonly VaccineService _vaccineService;
        private readonly IUnitOfWork _unitOfWork;

        public VaccinesController(VaccineService vaccineService, IUnitOfWork unitOfWork)
        {
            _vaccineService = vaccineService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVaccineDto dto)
        {
            if (dto == null) return BadRequest("Veri boş geldi.");

            // Sadece aşıyı ve veritabanı kaydını yapıyoruz.
            // Hatırlatıcı oluşturma mantığını sildik çünkü Flutter tarafı bunu zaten yapıyor.
            var result = await _vaccineService.CreateVaccineAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet("child/{childId}")]
        public async Task<IActionResult> GetByChildId(Guid childId)
        {
            var result = await _vaccineService.GetSchedulesByChildIdAsync(childId);
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("person/{personId}")]
        public async Task<IActionResult> GetByPersonId(Guid personId)
        {
            var result = await _vaccineService.GetByPersonIdAsync(personId);
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            var result = await _vaccineService.UpdateStatusAsync(id, status);
            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        // --- YENİ EKLENEN SİLME ENDPOINT'İ ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _vaccineService.DeleteVaccineAsync(id);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { success = true, message = "Aşı başarıyla silindi." });
        }
    }
}