namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultID { get; set; }
        public int ? RegistrationID { get; set; }
        public string ? Finish_Time { get; set; }
        public int ? Position { get; set; }
        public string ? Status { get; set; }
    }
}
