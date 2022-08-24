using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonesyHeist_App.Data;
using MonesyHeist_App.Data.Exceptions;
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

        [HttpGet("/{heistId}")]
        public async Task<IActionResult> GetHeistById(int heistId)
        {
            try
            {
                return Ok(_heistService.GetHeistById(heistId));
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

        [HttpGet("/{heistId}/members")]
        public async Task<IActionResult> GetHeistMembers(int heistId)
        {
            try
            {
                var members = await _heistService.GetHeistMembers(heistId);
                return Ok(members);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/{heistId}/skills")]
        public async Task<IActionResult> GetHeistSkills(int heistId)
        {
            try
            {
                var skills = await _heistService.GetHeistSkills(heistId);
                return Ok(skills);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/{heistId}/status")]
        public async Task<IActionResult> GetHeistStatus(int heistId)
        {
            try
            {
                var status = await _heistService.GetHeistStatus(heistId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("/{heistId}/outcome")]
        public async Task<IActionResult> GetHeistOutcome(int heistId)
        {
            try
            {
                var outcome = _heistService.GetHeistOutcome(heistId);
                return Ok(outcome);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (MethodNotAllowedException ex)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddHeist([FromBody] HeistVM heist)
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

        [HttpPut("/{heistId}/members")]
        public async Task<IActionResult> PutMembersInHeist(int heistId, List<string> members)
        {
            try
            {
                _heistService.PutMembersInHeist(heistId, members);
                return NoContent();
            }
            catch (Exception ex)
            {

                throw;
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

        [HttpPut("/{heistId}/status")]
        public async Task<IActionResult> StartHeist(int heistId)
        {
            try
            {
                await _heistService.StartHeist(heistId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
