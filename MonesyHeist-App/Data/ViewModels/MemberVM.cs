using MonesyHeist_App.Data.Model;

namespace MonesyHeist_App.Data.ViewModels
{
    public enum SexEnum
    {
        F,
        M
    }
    public enum StatusEnum
    {
        AVAILABLE,
        EXPIRED,
        INCARCARATED,
        RETIRED
    }

    public class MemberVM
    {
        public string Email { get; set; }

        public string Name { get; set; }
        public List<Skill> Skills { get; set; }
        public string? MainSkill { get; set; }

        public StatusEnum Status { get; set; }
        public SexEnum Sex { get; set; }

    }
}
