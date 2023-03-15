using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonesyHeist_App.Data.Exceptions;
using MonesyHeist_App.Data.Model;
using MonesyHeist_App.Data.ViewModels;
using System.Net.Mail;
using System.Net;

namespace MonesyHeist_App.Data.Services
{
    public class MemberService
    {
        private AppDbContext _context;
        private readonly IConfiguration _configuration;
        private List<string> SkillNames = new List<string>();
        private List<Skill> SkillList = new List<Skill>();

        public MemberService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            foreach (var item in _context.Skill.ToList())
            {
                SkillNames.Add(item.Name);
                SkillList.Add(item);
            }
        }

        public async Task<List<Member>> GetMembers()
        {
            List<Member> members = new List<Member>();

            members = _context.Members.Include(x => x.SkillsList).ToList();

            if (members.Count == 0) throw new NotFoundException("There are no members found.");

            return members;
        }

        public async Task<Member> AddMember (MemberVM member)
        {
            if (await GetMemberByEmail(member.Email) != null) throw new BadRequestException("Member with that email already exists.");

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
                Status = checkStatus(member.Status) ? member.Status : throw new Exception("Status not allowed!"),
                Sex = checkSex(member.Sex) ? member.Sex : throw new Exception("Gender not allowed!")
            };

            _context.Members.Add(_member);
            await _context.SaveChangesAsync();



            return _member;
        }

        public async Task<MemberSkillsVM> GetMemberSkills(int id)
        {
            MemberSkillsVM skills = new MemberSkillsVM();
            List<SkillsVM> memSkills = new List<SkillsVM>();
            skills.SkillsList = memSkills;
            var member = await GetMemberById(id);

            if (member == null) throw new NotFoundException("There is no member found!");

            foreach (var sk in _context.Skills.Where(x => x.MemberId == member.MemberId).ToList())
            {
                skills.SkillsList.Add(new SkillsVM()
                {
                    Level = sk.Level,
                    Name = sk.Skill.Name
                });
            }
            skills.MainSkill = member.MainSkill;

            return skills;
        }

        public async Task<List<Skills>> UpdateMemberSkills (int id, [FromBody]MemberSkillsVM skills)
        {
            List<Skills> _newList = new List<Skills>();
            bool hasMainSkill = false;

            var _member = _context.Members.FirstOrDefault(m => m.MemberId == id);

            if (_member == null) throw new NotFoundException("Member not found");

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
                    throw new BadRequestException("No skill found");
                }
            }

            _member.SkillsList = _context.Skills.Where(x => x.MemberId == _member.MemberId).ToList();
            _member.SkillsList.Clear();
            _member.SkillsList = _newList;

            if (hasMainSkill) _member.MainSkill = skills.MainSkill;
            else throw new BadRequestException("Main skill not in skills array");

            await _context.SaveChangesAsync();

            return _newList;
        }

        public async Task DeleteMemberSkill(int id, string skillName)  
        {
            var _mem = await GetMemberById(id);
            //var _mem = _context.Members.FirstOrDefault(m => m.MemberId == id);
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
        public async Task SendMembersEmail(string toEmail)
        {
            string EmailUsername = _configuration["AppSettings:EmailUsername"];
            string AppPassword = _configuration["AppSettings:AppPassword"];

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(EmailUsername, AppPassword);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(EmailUsername);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = "TestingSubject";
            message.Body = "I am sending you a testing message from a MoneyHeist app!";


            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't send email " + ex.Message);
            }
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

        private async Task<Member> GetMemberByEmail(string email)
        {
            return _context.Members.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<Member> GetMemberById(int id)
        {
            var mem = await _context.Members.Include(x => x.SkillsList).FirstOrDefaultAsync(x => x.MemberId == id);

            if (mem == null) throw new NotFoundException("Member not found.");

            return mem;
        }
    }
}
