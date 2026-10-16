namespace RaceDay.API.Models
{
    public class Race
    {
        public int RaceID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int Distance_km { get; set; } = 0;
        public string Type { get; set; } = string.Empty;


    }
}
