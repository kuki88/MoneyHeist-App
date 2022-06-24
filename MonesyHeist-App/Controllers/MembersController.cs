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
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MemberService _memberService;

        public MembersController(AppDbContext context, MemberService memberService)
        {
            _context = context;
            _memberService = memberService;
        }



        [HttpPost("member")]
        public IActionResult AddMember([FromBody]MemberVM member)
        {
            try
            {
                _memberService.AddMember(member);
                return Ok(member);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("member")]
        public IActionResult GetMembers()
        {
            var members = _memberService.GetMembers();
            return Ok(members);
        }
        [HttpPut("member/{memberId}/skills")]
        public IActionResult UpdateMemberSkills(int memberId, [FromBody]MemberSkillsVM skills)
        {
            try
            {
                var updateMember = _memberService.UpdateMemberSkills(memberId, skills);
                return NoContent();
            }
            catch (MainSkillException msEx)
            {
                return BadRequest(msEx.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("member/{member_id}/skills/{skill_name}")]
        public IActionResult DeleteMemberSkill(int member_id, string skill_name)
        {
            try
            {
                _memberService.DeleteMemberSkill(member_id, skill_name);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
