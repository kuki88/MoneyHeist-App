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

        public List<Heist> GetHeists()
        {
            return _context.Heists.ToList();
        }

        public Heist AddHeist(HeistVM heist)
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
                _context.SaveChanges();
                return _heist;
            }
            catch (Exception)
            {
                throw new Exception("Unexpected error");
            }

        }

        public void UpdateHeistSkills(int id, List<HeistSkillsVM> heistSkills)
        {
            List<HeistSkills> _heistSkills = new List<HeistSkills>();
            var _heist = _context.Heists.FirstOrDefault(m => m.HeistId == id);

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

            _context.SaveChanges();
        }
    }
}
