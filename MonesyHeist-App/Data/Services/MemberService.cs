using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonesyHeist_App.Data.Exceptions;
using MonesyHeist_App.Data.Model;
using MonesyHeist_App.Data.ViewModels;

namespace MonesyHeist_App.Data.Services
{
    public class MemberService
    {
        private AppDbContext _context;
        private List<string> SkillNames = new List<string>();
        private List<Skill> SkillList = new List<Skill>();

        public MemberService(AppDbContext context)
        {
            _context = context;
            foreach (var item in _context.Skill.ToList())
            {
                SkillNames.Add(item.Name);
                SkillList.Add(item);
            }
        }




        public async Task<List<Member>> GetMembers()
        {
            return _context.Members.Include(x => x.SkillsList).ToList();
        }

        public async Task<Member> AddMember (MemberVM member)
        {
            var skillList = new List<Skills>();

            var _member = new Member()
            {
                Email = member.Email,
                Name = member.Name,
                SkillsList = member.SkillsList.Select(x => new Skills()
                {
                    Skill = SkillList.FirstOrDefault(s => s.Name == x.Name),
                    Level = x.Level,
                }).ToList(),
                MainSkill = member.MainSkill,
                Status = checkStatus(member.Status)? member.Status : throw new Exception("Status not allowed!"),
                Sex = checkSex(member.Sex) ? member.Sex : throw new Exception("Sex not allowed!")
            };

            _member.SkillsList = skillList;

            _context.Members.Add(_member);
            await _context.SaveChangesAsync();

            return _member;
        }

        public async Task<List<Skills>> UpdateMemberSkills (int id, [FromBody]MemberSkillsVM skills)
        {
            List<Skills> _newList = new List<Skills>();
            bool hasMainSkill = false;

            var _member = _context.Members.FirstOrDefault(m => m.MemberId == id);

            if (_member == null) throw new Exception("Member not found");

            foreach (var skillVM in skills.SkillsList)
            {
                if (skillVM.Name == skills.MainSkill) hasMainSkill = true;


                if (SkillNames.Contains(skillVM.Name))
                {
                    try
                    {
                        var _skill = _context.Skill.FirstOrDefault(s => s.Name == skillVM.Name);
                        _newList.Add(new Skills()
                        {
                            Level = skillVM.Level,
                            MemberId = id,
                            Member = _member,
                            Skill = _skill,
                            SkillId = _skill.SkillId,
                        });
                    }
                    catch (Exception)
                    {
                        throw new Exception("Unexpected error");
                    }
                }
                else
                {
                    throw new Exception("No skill found");
                }
            }

            if (hasMainSkill) _member.MainSkill = skills.MainSkill;
            else throw new MainSkillException("Main skill not in skills array");

            await _context.SaveChangesAsync();

            return _newList;
        }

        public async Task DeleteMemberSkill(int id, string skillName)  
        {
            var _mem = _context.Members.FirstOrDefault(m => m.MemberId == id);
            if (_mem != null)
            {
                try
                {
                    _mem.SkillsList.Remove(_mem.SkillsList.FirstOrDefault(s => s.Skill.Name == skillName));
                }
                catch (Exception)
                {
                    throw new Exception("Skill not found!");
                }
            }
            else
            {
                throw new Exception("Member not found!");
            }

            await _context.SaveChangesAsync();
        }

        private static bool checkStatus(string status)
        {
            foreach (var st in Global._statusList)
            {
                if (status.ToUpper() == st) return true;
            }
            return false;
        }

        private static bool checkSex(char sex)
        {
            foreach (var se in Global._sexList)
            {
                if (sex == se) return true;
            }
            return false;
        }
    }
}
