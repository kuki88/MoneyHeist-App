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
    public class MemberController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MemberService _memberService;

        public MemberController(AppDbContext context, MemberService memberService)
        {
            _context = context;
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody]MemberVM member)
        {
            try
            {
                await _memberService.AddMember(member);
                return Ok(member);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _memberService.GetMembers();
            return Ok(members);
        }

        [HttpGet("{memberId}/skills")]
        public async Task<IActionResult> GetMemberSkills(int memberId)
        {
            try
            {
                return Ok(_memberService.GetMemberSkills(memberId));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{memberId}/skills")]
        public async Task<IActionResult> UpdateMemberSkills(int memberId, [FromBody]MemberSkillsVM skills)
        {
            try
            {
                var updateMember = await _memberService.UpdateMemberSkills(memberId, skills);
                return NoContent();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{member_id}/skills/{skill_name}")]
        public async Task<IActionResult> DeleteMemberSkill(int member_id, string skill_name)
        {
            try
            {
                await _memberService.DeleteMemberSkill(member_id, skill_name);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
