using System.ComponentModel.DataAnnotations;

namespace MonesyHeist_App.Data.Model
{
    public class HeistSkills
    {
        [Key]
        public int HeistSkillId { get; set; }
        public Skill Skill { get; set; }
        public string Level { get; set; }
        public int Members { get; set; }
        public int HeistId { get; set; } 
        public Heist Heist { get; set; }
    }
}
