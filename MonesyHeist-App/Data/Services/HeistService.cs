using Microsoft.EntityFrameworkCore;
using MonesyHeist_App.Data.Exceptions;
using MonesyHeist_App.Data.Model;
using MonesyHeist_App.Data.ViewModels;

namespace MonesyHeist_App.Data.Services
{
    public class HeistService
    {
        private AppDbContext _context;

        public HeistService(AppDbContext context)
        {
            _context = context;
            foreach (var heist in _context.Heists.ToList())
            {
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
        }

        public async Task<List<Heist>> GetHeists()
        {
            List<Heist> list = await _context.Heists.Include(x => x.Skills).ToListAsync();

            if (list.Count == 0) throw new NotFoundException("There are no Heists in database!");
            return list;
        }
        public async Task<List<Member>> GetEligibleMembers(int heistId)
        {
            var members = await _context.Members.Include(x => x.SkillsList).Where(x => x.Status == "AVAILABLE" || x.Status == "RETIRED").ToListAsync();
            var hei = await GetHeistById(heistId);


            return MatchingMembers(members, hei);
        }
        public async Task<string> GetHeistOutcome(int heistId)
        {
            var heist = await GetHeistById(heistId);

            if (heist.Status.ToUpper() != "FINISHED") throw new MethodNotAllowedException("Heist is not finished!");

            var heistMembers = _context.HeistMembers.Where(m => m.HeistId == heist.HeistId).ToList();
            int requiredNumber = heist.Skills.Count;

            return Outcome(heist, heistMembers, requiredNumber);
        }
        public async Task<Heist> AddHeist(HeistVM heist)
        {
            List<string> _heistNames = _context.Heists.Select(x => x.Name).ToList();

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

                    heistSkill.Skill = _context.Skill.FirstOrDefault(s => s.Name == heSkill.Skill);
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
            catch (BadRequestException ex)
            {
                throw new BadRequestException("Unexpected error" + ex);
            }

        }
        public async Task<string> GetHeistStatus(int heistId)
        {
            var heist = await GetHeistById(heistId);
            return heist.Status;
        }
        public async Task<List<HeistSkillsVM>> GetHeistSkills(int heistId)
        {
            try
            {
                var heist = await GetHeistById(heistId);

                var skills = heist.Skills.ToList();

                List<HeistSkillsVM> heistSkillsVMs = heist.Skills.ToList().Select(x => new HeistSkillsVM()
                {
                    Level = x.Level,
                    Skill = x.Skill.Name,
                    Members = x.Members
                }).ToList();

                if (heistSkillsVMs.Count == 0) throw new NotFoundException("There are no Heist skills found!");

                return heistSkillsVMs;
            }
            catch (NotFoundException ex)
            {
                throw ex;
            }

        }
        public async Task StartHeist(int id)
        {
            var hei = await GetHeistById(id);

            hei.Status = "IN_PROGRESS";
            await _context.SaveChangesAsync();
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

            if (memberInfos.Count == 0) throw new NotFoundException("There are no members found!");

            return memberInfos;
        }
        public async Task UpdateHeistSkills(int id, List<HeistSkillsVM> heistSkills)
        {
            List<HeistSkills> _heistSkills = new List<HeistSkills>();
            var _heist = await GetHeistById(id);


            if (_heist.Status.ToUpper() == "IN_PROGRESS") throw new MethodNotAllowedException("The heist has already started!");

            foreach (var heist in heistSkills)
            {
                try
                {
                    HeistSkills skill = new HeistSkills();
                    skill.Skill = _context.Skill.FirstOrDefault(s => s.Name == heist.Skill);
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
            var heist = await GetHeistById(heistId);

            if (heist.Status.ToUpper() != "PLANNING") throw new MethodNotAllowedException("Heist status is not \"PLANNING\".");


            var membersDb = _context.Members.ToList();
            foreach (var mem in members)
            {
                var member = MemberExists(mem);

                if (!IsAvailable(member)) throw new BadRequestException("Member is not available for this heist.");

                if (member == null) throw new NotFoundException("Member " + mem + " does not exist");
                
                if (!HasRequiredSkill(member, heist)) throw new BadRequestException("Member + " + member.Name + " does not have required skills!");

                _context.HeistMembers.Add(new HeistMembers()
                {
                    Heist = heist,
                    Member = member,
                });
            }

            heist.Status = "Ready";
            await _context.SaveChangesAsync();
        }
        public async Task<Heist> GetHeistById(int id)
        {
            var heist = await _context.Heists.Include(x => x.Skills).ThenInclude(x => x.Skill).FirstOrDefaultAsync(x => x.HeistId == id);

            if (heist == null) throw new NotFoundException("There is no heist found.");

            return heist;
        }
        private bool HasRequiredSkill(Member mem, Heist heist)
        {
            foreach (var Skill in heist.Skills)
            {
                foreach (var memSkill in mem.SkillsList)
                {
                    if (memSkill.SkillId == Skill.SkillId && memSkill.Level.Length >= Skill.Level.Length)
                    {
                        return true;
                    }
                }
            }
            return false;
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

            if (matchingMembers.Count == 0) throw new NotFoundException("There are no matching members found.");

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
            if (count == 0) throw new NotFoundException("There is no members registered for this heist!");

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
            if (matchingMembers.Count / count >= 0.75 && matchingMembers.Count / count < 1)
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

            if (matchingMembers.Count / count == 1)
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
