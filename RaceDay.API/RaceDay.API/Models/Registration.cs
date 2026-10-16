namespace RaceDay.API.Models
{
    public class Registration
    {
        public int RegistrationID { get; set; }
        public int ? Bib_Number { get; set; }
        public string ? status { get; set; }
        public DateTime ? RegistrationDate { get; set; }
        public int ? ParticipantID { get; set; }
        public int ? RaceID { get; set; }

    }
}
