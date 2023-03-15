namespace MonesyHeist_App.Data

{
    public class Global
    {
        public static readonly List<string> _statusList = new List<string>()
        {
            "AVAILABLE",
            "EXPIRED",
            "INCARCERATED",
            "RETIRED"
        };
        public static readonly List<char> _sexList = new List<char>()
        {
            'F',
            'M'
        };
    }
}
