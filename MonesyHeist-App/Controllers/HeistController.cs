using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonesyHeist_App.Data;
using MonesyHeist_App.Data.Services;
using MonesyHeist_App.Data.ViewModels;

namespace MonesyHeist_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeistController : ControllerBase
    {
        private readonly AppDbContext _context;
        public HeistService _heistService;

        public HeistController(AppDbContext context, HeistService heistService)
        {
            _context = context;
            _heistService = heistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHeists()
        {
            return Ok(await _heistService.GetHeists());
        }

        [HttpPost]
        public async Task<IActionResult> AddHeist([FromBody]HeistVM heist)
        {
            try
            {
                var _heist = await _heistService.AddHeist(heist);
                return Ok(_heist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("/{id}/skills")]
        public async Task<IActionResult> UpdateHeistSkills(int id, [FromBody] List<HeistSkillsVM> heistSkills)
        {
            try
            {
                await _heistService.UpdateHeistSkills(id, heistSkills);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/{heistId}/eligible_members")]
        public async Task<IActionResult> GetEligibleMembers(int heistId)
        {
            try
            {
                var list = await _heistService.GetEligibleMembers(heistId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
