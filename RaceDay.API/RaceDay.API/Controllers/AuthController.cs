
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(
            AppDbContext db,
            JwtService jwtService,
            IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                    new { message = "Name, email and password are required." });
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(
                    new { message = "Password must be at least 8 characters." });
            }

            var email = request.Email.Trim();

            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                return Conflict(
                    new { message = "An account with this email already exists." });
            }

            var user = new User
            {
                Name = request.Name.Trim(),
                Email = email,
                Contact_Info = request.Contact_Info?.Trim() ?? string.Empty,

                // Public registration creates Participants only.
                User_Type = "Participant"
            };

            user.Password = _passwordHasher.HashPassword(
                user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                user.UserID,
                user.Name,
                user.Email,
                role = user.User_Type
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            var email = request.Email.Trim();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(
                    new { message = "Invalid email or password." });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user, user.Password, request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(
                    new { message = "Invalid email or password." });
            }

            var token = _jwtService.GenerateToken(
                user.UserID,
                user.Email,
                user.User_Type);

            return Ok(new
            {
                message = "Login successful.",
                token,
                userId = user.UserID,
                email = user.Email,
                role = user.User_Type
            });
        }
    }
}
