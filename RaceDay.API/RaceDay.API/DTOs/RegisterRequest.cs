
namespace RaceDay.API.DTOs
{
    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Contact_Info { get; set; } = string.Empty;
        public string User_Type { get; set; } = string.Empty;
    }
}
