using MonesyHeist_App.Data.Model;
using MonesyHeist_App.Data.ViewModels;

namespace MonesyHeist_App.Data.Services
{
    public class MemberService
    {
        private AppDbContext _context;

        public MemberService(AppDbContext context)
        {
            _context = context;
        }
        
        public List<Member> GetMembers()
        {
            return _context.Members.ToList();
        }

        public Member AddMember (MemberVM member)
        {
            var _member = new Member()
            {
                Email = member.Email,
                Name = member.Name,
                Skills = member.Skills,
                MainSkill = member.MainSkill,
                Status = (Model.StatusEnum)member.Status,
                Sex = (Model.SexEnum)member.Sex
            };

            _context.Members.Add(_member);
            _context.SaveChanges();

            return _member;
        }
    }
}
