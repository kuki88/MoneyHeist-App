using MonesyHeist_App.Data.Model;

namespace MonesyHeist_App.Data.ViewModels
{
    public class MemberVM
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public List<SkillsVM> SkillsList { get; set; }
        public string? MainSkill { get; set; }

        public string Status { get; set; }
        public char Sex { get; set; }

    }
}
