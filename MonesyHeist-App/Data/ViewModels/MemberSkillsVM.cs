using MonesyHeist_App.Data.Model;

namespace MonesyHeist_App.Data.ViewModels
{
    public class MemberSkillsVM
    {
        public List<SkillsVM> SkillsList { get; set; }
        public string? MainSkill { get; set; }
    }
}
