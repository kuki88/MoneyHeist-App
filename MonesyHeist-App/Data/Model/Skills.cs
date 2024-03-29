﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonesyHeist_App.Data.Model
{
    public class Skills
    {
        [Key]
        public int SkillsId { get; set; }

        [ForeignKey(nameof(Member))]
        public int MemberId { get; set; }
        public virtual Member Member { get; set; }

        [ForeignKey(nameof(Skill))]
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }
        public string Level { get; set; }
    }
}
