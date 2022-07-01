using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonesyHeist_App.Data.Model
{
    public class HeistSkills
    {
        [Key]
        public int HeistSkillId { get; set; }
        [ForeignKey(nameof(Skill))]
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }
        public string Level { get; set; }
        public int Members { get; set; }
        [ForeignKey(nameof(Heist))]
        public int HeistId { get; set; }
        public virtual Heist Heist { get; set; }
    }
}
