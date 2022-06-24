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
        public IActionResult GetHeists()
        {
            return Ok(_heistService.GetHeists());
        }

        [HttpPost("heist")]
        public IActionResult AddHeist([FromBody]HeistVM heist)
        {
            try
            {
                var _heist = _heistService.AddHeist(heist);
                return Ok(_heist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("heist/{id}/skills")]
        public IActionResult UpdateHeistSkills(int id, [FromBody] List<HeistSkillsVM> heistSkills)
        {
            try
            {
                _heistService.UpdateHeistSkills(id, heistSkills);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
