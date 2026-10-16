namespace RaceDay.API.Models
{
    public class Participant
    {
        public int ParticipantID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string ? EmergencyContact { get; set; } 
        public string ? TeamName { get; set; }
    }
}
