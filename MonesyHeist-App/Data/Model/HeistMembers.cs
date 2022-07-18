using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonesyHeist_App.Data.Model
{
    public class HeistMembers
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Heist))]
        public int HeistId { get; set; }
        public virtual Heist Heist { get; set; }
        [ForeignKey(nameof(Member))]
        public int MemberId { get; set; }
        public virtual Member Member { get; set; }
    }
}
