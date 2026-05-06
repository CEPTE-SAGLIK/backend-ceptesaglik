using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HealthApp.Business.DTOs;
using HealthApp.Business.Services;

namespace HealthApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AllergiesController : ControllerBase
    {
        private readonly AllergyService _allergyService;

        public AllergiesController(AllergyService allergyService)
        {
            _allergyService = allergyService;
        }

        // 1. Tüm Alerjileri Listeleme (GET)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _allergyService.GetAllAllergiesAsync();
            return Ok(result);
        }

        // 1.1 Sadece Seçilen Kişinin Alerjilerini Listeleme (GET by PersonId)
        [HttpGet("person/{personId}")]
        public async Task<IActionResult> GetAllByPersonId(Guid personId)
        {
            // Sadece bu PersonId'ye ait olanlar
            var result = await _allergyService.GetAllByPersonIdAsync(personId); 
            return Ok(result);
        }

        // 2. Yeni Alerji Ekleme (POST)
        [HttpPost]
        public async Task<IActionResult> AddAllergy([FromBody] AllergyCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _allergyService.AddAllergyAsync(createDto);

            return CreatedAtAction(nameof(AddAllergy), new { id = result.Id }, result);
        }

        // 3. Alerji Silme (DELETE) 🗑️
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllergy(Guid id)
        {
            var result = await _allergyService.DeleteAllergyAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Silinmek istenen alerji bulunamadı." });
            }

            return Ok(new { message = "Alerji başarıyla silindi." });
        }
    }
}