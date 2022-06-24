namespace MonesyHeist_App.Data.ViewModels
{
    public class HeistVM
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<HeistSkillsVM> Skills { get; set; }

    }
}
