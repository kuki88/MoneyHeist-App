using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MonesyHeist_App.Data.Model
{
    public class Member
    {
        public int MemberId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<Skills> SkillsList { get; set; }
        public string? MainSkill { get; set; }
        public string Status { get; set; }
        public char Sex { get; set; }
    }
}
