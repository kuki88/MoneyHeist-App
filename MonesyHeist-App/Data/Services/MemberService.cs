using Microsoft.AspNetCore.Mvc;
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




        public List<Member> GetMembers()
        {
            return _context.Members.ToList();
        }

        public Member AddMember (MemberVM member)
        {
            var skillList = new List<Skills>();

            var _member = new Member()
            {
                Email = member.Email,
                Name = member.Name,
                //SkillsList = member.SkillsList,
                MainSkill = member.MainSkill,
                Status = (Model.StatusEnum)member.Status,
                Sex = (Model.SexEnum)member.Sex
            };

            foreach (var skill in member.SkillsList)
            {
                skillList.Add(new Skills()
                {
                    Member = _member,
                    Skill = SkillList.FirstOrDefault(s => s.Name == skill.Name),
                    Level = skill.Level,
                });
            }

            _member.SkillsList = skillList;

            _context.Members.Add(_member);
            _context.SaveChanges();

            return _member;
        }

        public List<Skills> UpdateMemberSkills (int id, [FromBody]MemberSkillsVM skills)
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

            _context.SaveChanges();

            return _newList;
        }

        public void DeleteMemberSkill(int id, string skillName)  
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

            _context.SaveChanges();
        }
    }
}
