using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MonesyHeist_App.Data.Model
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
    public class Member
    {
        public int MemberId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<Skills> SkillsList { get; set; }
        public string? MainSkill { get; set; }
        public StatusEnum Status { get; set; }
        public SexEnum Sex { get; set; }
    }
}
