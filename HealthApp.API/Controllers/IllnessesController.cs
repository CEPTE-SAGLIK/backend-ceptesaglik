using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using HealthApp.Business.DTOs;
using HealthApp.Business.Services;

namespace HealthApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IllnessesController : ControllerBase
    {
        private readonly IllnessService _illnessService;

        public IllnessesController(IllnessService illnessService)
        {
            _illnessService = illnessService;
        }

        // 1. Tüm Hastalıkları Listeleme (GET)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _illnessService.GetAllIllnessesAsync();
            return Ok(result);
        }

        // 1.1 Sadece Seçilen Kişinin Hastalıklarını Listeleme (GET by PersonId)
        [HttpGet("person/{personId}")]
        public async Task<IActionResult> GetAllByPersonId(Guid personId)
        {
            var result = await _illnessService.GetAllByPersonIdAsync(personId); 
            return Ok(result);
        }

        // 2. Yeni Hastalık Ekleme (POST)
        [HttpPost]
        public async Task<IActionResult> AddIllness([FromBody] IllnessCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _illnessService.AddIllnessAsync(createDto);

            return CreatedAtAction(nameof(AddIllness), new { id = result.Id }, result);
        }

        // 3. Hastalık Silme (DELETE) - YENİ EKLEDİĞİMİZ KISIM 🗑️
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIllness(Guid id)
        {
            var result = await _illnessService.DeleteIllnessAsync(id);

            if (!result)
            {
                // Eğer silinecek kayıt bulunamazsa 404 hatası döner
                return NotFound(new { message = "Silinmek istenen hastalık bulunamadı." });
            }

            // Başarıyla silindiğinde 200 OK döner
            return Ok(new { message = "Hastalık başarıyla silindi." });
        }
    }
}