using Microsoft.EntityFrameworkCore;
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
            List<Member> _list = new List<Member>();
            var hei = GetHeistById(heistId);

            if (hei == null) throw new Exception("Heist does not exist!");

            var members = _context.Members.Where(x => x.Status == "AVAILABLE" || x.Status == "RETIRED").ToList();
            foreach (var mem in members)
            {
                foreach (var sk in mem.SkillsList)
                {
                    foreach (var hSk in hei.Skills)
                    {
                        if (sk.Skill == hSk.Skill && sk.Level.Count() >= hSk.Level.Count())
                        {
                            _list.Add(mem);
                        }
                    }
                }
            }

            return _list;
        }


        public async Task<Heist> AddHeist(HeistVM heist)
        {
            if (heist.StartTime > heist.EndTime)
            {
                throw new Exception("Start time can't be greater than end time");
            }

            if (_heistNames.Contains(heist.Name))
            {
                throw new Exception("Heist with that name already exists");
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
                    Skills = _heistSkills
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

        public async Task UpdateHeistSkills(int id, List<HeistSkillsVM> heistSkills)
        {
            List<HeistSkills> _heistSkills = new List<HeistSkills>();
            var _heist = GetHeistById(id);

            if (_heist == null) throw new Exception("Heist does not exist");

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

        private Heist GetHeistById(int id)
        {
            return _context.Heists.FirstOrDefault(x => x.HeistId == id);
        }
    }
}
