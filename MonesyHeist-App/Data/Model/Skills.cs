using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonesyHeist_App.Data.Model
{
    public class Skills
    {
        [Key]
        public int SkillsId { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public string Level { get; set; }
    }
}
