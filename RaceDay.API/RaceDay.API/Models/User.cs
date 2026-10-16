namespace RaceDay.API.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Contact_Info { get; set; } = string.Empty;
        public string User_Type { get; set; } = string.Empty;

    }
}
