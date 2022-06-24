namespace MonesyHeist_App.Data.Model
{
    public class Heist
    {
        public int HeistId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<HeistSkills> Skills { get; set; }
    }
}
