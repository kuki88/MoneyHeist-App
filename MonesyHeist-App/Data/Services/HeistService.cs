﻿using Microsoft.EntityFrameworkCore;
using MonesyHeist_App.Data.Exceptions;
using MonesyHeist_App.Data.Model;
using MonesyHeist_App.Data.ViewModels;

namespace MonesyHeist_App.Data.Services
{
    public class HeistService
    {
        private AppDbContext _context;

        private List<string> _heistNames = new List<string>();
        private List<string> _skillName = new List<string>();
        private List<Heist> _heistList = new List<Heist>();
        private List<Skill> _skillList = new List<Skill>();
        //private List<string> _statusList = new List<string>()
        //{
        //    "AVAILABLE",
        //    "EXPIRED",
        //    "INCARCERATED",
        //    "RETIRED"
        //};

        public HeistService(AppDbContext context)
        {
            _context = context;
            foreach (var heist in _context.Heists.ToList())
            {
                _heistNames.Add(heist.Name);
                _heistList.Add(heist);

                if (heist.StartTime < DateTime.Now && heist.EndTime > DateTime.Now)
                {
                    heist.Status = "IN_PROGRESS";
                    _context.SaveChanges();
                }

                if (DateTime.Now > heist.EndTime)
                {
                    heist.Status = "FINISHED";
                    _context.SaveChanges();
                }
            }
            foreach (var skill in _context.Skill.ToList()) 
            {
                _skillList.Add(skill);
                _skillName.Add(skill.Name);
            }

        }

        public async Task<List<Heist>> GetHeists()
        {
            return _context.Heists.Include(x => x.Skills).ToList();
        }
        public async Task<List<Member>> GetEligibleMembers(int heistId)
        {
            var members = _context.Members.Where(x => x.Status == "AVAILABLE" || x.Status == "RETIRED").ToList();
            var hei = GetHeistById(heistId);

            if (hei == null) throw new NotFoundException("Heist not found!");

            return MatchingMembers(members, hei);
        }

        public async Task<string> GetHeistOutcome(int heistId)
        {
            string outcome = "unknown";
            var heist = GetHeistById(heistId);
            
            if (heist == null) throw new NotFoundException("Heist does not exist!");
            if (heist.Status.ToUpper() != "FINISHED") throw new MethodNotAllowedException("Heist is not finished!");

            var heistMembers = _context.HeistMembers.Where(m => m.HeistId == heist.HeistId).ToList();
            int requiredNumber = heist.Skills.Count;

            return Outcome(heist, heistMembers, requiredNumber);
        }


        public async Task<Heist> AddHeist(HeistVM heist)
        {
            if (heist.StartTime > heist.EndTime)
            {
                throw new BadRequestException("Start time can't be greater than end time");
            }
            
            if (_heistNames.Contains(heist.Name))
            {
                throw new BadRequestException("Heist with that name already exists");
            }

            try
            {
                List<HeistSkills> _heistSkills = new List<HeistSkills>();
                var _heist = new Heist()
                {
                    Name = heist.Name,
                    Location = heist.Location,
                    StartTime = heist.StartTime,
                    EndTime = heist.EndTime,
                    Skills = _heistSkills,
                    Status = "Planning"
                };

                foreach (var heSkill in heist.Skills)
                {
                    var heistSkill = new HeistSkills();

                    heistSkill.Skill = _skillList.FirstOrDefault(s => s.Name == heSkill.Skill);
                    heistSkill.Level = heSkill.Level;
                    heistSkill.Members = heSkill.Members;
                    heistSkill.Heist = _heist;
                    _heistSkills.Add(heistSkill);
                    _context.HeistSkills.Add(heistSkill);
                }

                _heist.Skills = _heistSkills;

                _context.Heists.Add(_heist);
                await _context.SaveChangesAsync();
                return _heist;
            }
            catch (Exception)
            {
                throw new Exception("Unexpected error");
            }

        }

        public async Task<string> GetHeistStatus(int heistId)
        {
            var heist = GetHeistById(heistId);
            return heist.Status;
        }

        public async Task<List<HeistSkillsVM>> GetHeistSkills(int heistId)
        {
            var heist = GetHeistById(heistId);

            List<HeistSkillsVM> heistSkillsVMs = heist.Skills.ToList().Select(x => new HeistSkillsVM()
            {
                Level = x.Level,
                Skill = x.Skill.Name,
                Members = x.Members
            }).ToList();

            return heistSkillsVMs;
        }

        public async Task StartHeist(int id)
        {
            var hei = GetHeistById(id);
            hei.Status = "IN_PROGRESS";
            _context.SaveChangesAsync();
        }

        public async Task<List<MemberInfoVM>> GetHeistMembers(int heistId)
        {
            var heistMem = _context.HeistMembers.Where(hm => hm.HeistId == heistId).ToList();
            List<MemberInfoVM> memberInfos = new List<MemberInfoVM>();
            foreach (var mem in heistMem)
            {
                memberInfos.Add(new MemberInfoVM()
                {
                    Name = mem.Member.Name,
                    Skills = mem.Member.SkillsList.Select(x => new SkillsVM()
                    {
                        Level = x.Level,
                        Name = x.Member.Name,
                    }).ToList()
                });
            }

            return memberInfos;
        }

        
        public async Task UpdateHeistSkills(int id, List<HeistSkillsVM> heistSkills)
        {
            List<HeistSkills> _heistSkills = new List<HeistSkills>();
            var _heist = GetHeistById(id);

            if (_heist== null)
            {
                throw new NotFoundException("Heist does not exist!");
            }

            if (_heist.Status.ToLower() == "IN_PROGRESS".ToLower())
            {
                throw new MethodNotAllowedException("The heist has already started!");
            }

            foreach (var heist in heistSkills)
            {
                try
                {
                    HeistSkills skill = new HeistSkills();
                    skill.Skill = _skillList.FirstOrDefault(s => s.Name == heist.Skill);
                    skill.Heist = _heist;
                    skill.HeistId = _heist.HeistId;
                    skill.Level = heist.Level;
                    skill.Members = heist.Members;
                    _heistSkills.Add(skill);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unexpected exception: " + ex.Message);
                }
            }

            foreach (var skill in _heist.Skills)
            {
                _context.HeistSkills.Remove(skill);
            }

            foreach (var skill in _heistSkills)
            {
                _context.HeistSkills.Add(skill);
            }

            _heist.Skills = _heistSkills;

            await _context.SaveChangesAsync();
        }
        public async Task PutMembersInHeist(int heistId, List<string> members)
        {
            var heist = GetHeistById(heistId);

            if (heist == null) throw new NotFoundException("Heist does not exist!");

            if (heist.Status.ToUpper() != "PLANNING") throw new MethodNotAllowedException("Heist status is not \"PLANNING\".");


            var membersDb = _context.Members.ToList();
            foreach (var mem in members)
            {
                var member = MemberExists(mem);

                if (!IsAvailable(member)) throw new BadRequestException("Member is not available for this heist.");

                if (member != null)
                {
                    if (!HasRequiredSkill(member, heist))
                    {
                        throw new BadRequestException("Member + " + member.Name + " does not have required skills!");
                    }

                    _context.HeistMembers.Add(new HeistMembers()
                    {
                        Heist = heist,
                        Member = member,
                    });
                }
                else throw new NotFoundException("Member does not exist!");
            }

            heist.Status = "Ready";
            await _context.SaveChangesAsync();
        }

        private bool HasRequiredSkill(Member mem, Heist heist)
        {
            foreach (var Skill in heist.Skills)
            {
                foreach (var memSkill in mem.SkillsList)
                {
                    if (memSkill.Skill == Skill.Skill && memSkill.Level.Length >= Skill.Level.Length)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Heist GetHeistById(int id)
        {
            var heist = _context.Heists.FirstOrDefault(x => x.HeistId == id);

            if (heist == null) throw new Exception("Heist does not exist!");

            return heist;
        }

        private Member MemberExists(string memberName)
        {
            foreach (var memberDb in _context.Members)
            {
                if (memberDb.Name == memberName)
                {
                    return memberDb;
                }
            }
            return null;
        }

        private bool IsAvailable(Member member)
        {
            if (member.Status.ToUpper() == "AVAILABLE" || member.Status.ToUpper() == "RETIRED") return true;

            return false;
        }

        private List<Member> MatchingMembers(List<Member> members, Heist heist)
        {
            List<Member> matchingMembers = new List<Member>();

            foreach (var mem in members)
            {
                if (HasRequiredSkill(mem, heist))
                {
                    matchingMembers.Add(mem);
                }
            }

            return matchingMembers;
        }

        private string Outcome(Heist heist, List<HeistMembers> heistMembers, int numberOfHeistSkills)
        {
            var ran = new Random();
            List<int> randomList = new List<int>();

            string outcome = "unknown";
            List<Member> members = heistMembers.Select(x => x.Member).ToList();
            List<Member> matchingMembers = MatchingMembers(members, heist);
            var mems = _context.Members.ToList();

            int count = _context.HeistMembers.Where(h => h.HeistId == heist.HeistId).ToList().Count;

            if (matchingMembers.Count / count < 0.5)
            {
                foreach (var mem in heistMembers)
                {
                    _context.Members.FirstOrDefault(x => x.MemberId == mem.MemberId).Status = RandomBool() ? "EXPIRED" : "INCARCERATED"; 
                }
                outcome = "FAILED";
            }
            if (matchingMembers.Count / count >= 0.5 && matchingMembers.Count / count < 0.75)
            {
                if (RandomBool() == false)
                {
                    int num = 0;

                    for (int i = 0; i < (2 * members.Count) / 3; i++)
                    {
                        do
                        {
                            num = RandomNumber(members.Count);
                        } while (randomList.Contains(num));

                        _context.Members.FirstOrDefault(x => x.MemberId == heistMembers[num].MemberId).Status = RandomBool() ? "EXPIRED" : "INCARCERATED";
                    }

                    outcome = "FAILED";
                    randomList.Clear();
                }
                else
                {
                    int num = 0;

                    for (int i = 0; i < members.Count / 3; i++)
                    {
                        do
                        {
                            num = RandomNumber(members.Count);
                        } while (randomList.Contains(num));
                        _context.Members.FirstOrDefault(x => x.MemberId == heistMembers[num].MemberId).Status = RandomBool() ? "EXPIRED" : "INCARCERATED";
                    }

                    outcome = "SUCCEEDED";
                    randomList.Clear();
                }
            }
            if(matchingMembers.Count / count >= 0.75 && matchingMembers.Count / count < 1)
            {
                int num = 0;

                for (int i = 0; i < members.Count / 3; i++)
                {
                    do{
                        num = RandomNumber(members.Count);
                    } while (randomList.Contains(num)) ;
                    _context.Members.FirstOrDefault(x => x.MemberId == heistMembers[num].MemberId).Status = "INCARCERATED";
                }

                outcome = "SUCCEEDED";
            }

            if(matchingMembers.Count / count == 1)
            {
                outcome = "SUCCEEDED";
            }

            return outcome;
        }

        private int RandomNumber(int granica)
        {
            var ran = new Random();
            return ran.Next(0, granica);
        }

        private bool RandomBool()
        {
            var random = new Random();

            return random.Next(2) == 1;
        }
    }
}
