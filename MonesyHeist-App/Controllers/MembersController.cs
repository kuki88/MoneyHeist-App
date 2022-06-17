using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonesyHeist_App.Data;
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

        [HttpGet("member")]
        public IActionResult GetMembers()
        {
            var members = _memberService.GetMembers();
            return Ok(members);
        }

        [HttpPost("member")]
        public IActionResult AddMember([FromBody]MemberVM member)
        {
            _memberService.AddMember(member);
            return Ok();
        }
    }
}
