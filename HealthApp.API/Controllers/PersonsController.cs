using HealthApp.Business.DTOs;
using HealthApp.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonsController : ControllerBase
    {
        private readonly PersonService _personService;

        public PersonsController(PersonService personService)
        {
            _personService = personService;
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Token içinde kullanıcı kimliği bulunamadı.");
            }

            return Guid.Parse(userId);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson(PersonCreateDTO dto)
        {
            try
            {
                var created = await _personService.CreatePersonAsync(dto, GetUserId());
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/Persons/profile — Flutter: POST /api/persons/profile (ilk profil oluşturma)
        [HttpPost("profile")]
        public async Task<IActionResult> CreateProfile(PersonCreateDTO dto)
        {
            try
            {
                var created = await _personService.CreatePersonAsync(dto, GetUserId());
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPersons()
        {
            var persons = await _personService.GetPersonsByUserIdAsync(GetUserId());
            return Ok(persons);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPersonById(Guid id)
        {
            try
            {
                var person = await _personService.GetPersonByIdAsync(id, GetUserId());
                return Ok(person);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePerson(Guid id, PersonUpdateDTO dto)
        {
            try
            {
                var updated = await _personService.UpdatePersonAsync(id, dto, GetUserId());
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
