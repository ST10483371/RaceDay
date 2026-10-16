
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RaceDay.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //identifies the user, email and role(organizer or participant)
        public string GenerateToken(
            int userId,
            string email,
            string role)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is missing.");

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(ClaimTypes.Email, email),

                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: "RaceDay.API",
                audience: "RaceDay.Client",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), //makes the token expire after two hours
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token); //converts the token into the
                                    //long string the client recieves after logging in
        }
    }
}
