using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using SAVE_ANIMLS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SAVE_ANIMLS.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _configuration;

        public AuthController(IMongoDatabase database, IConfiguration configuration)
        {
            _users = database.GetCollection<User>("users");
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Check if the user exists in the database
            var user = _users.Find(u => u.Email == loginRequest.Email && u.Password == loginRequest.Password).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Invalid email or password");
            }
            if (!user.IsApproved)
            {
                return BadRequest("Account not approved");
            }
            // Create claims for the user
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

            // Generate JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), // Set the token expiration time
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("register")]
        public ActionResult<User> CreateUser(User user)
        {
            // Check if the email is already used
            var existingUser = _users.Find(u => u.Email == user.Email).FirstOrDefault();
            if (existingUser != null)
            {
                return BadRequest("Email is already in use");
            }

            user.Id = ObjectId.GenerateNewId();
            user.IsApproved = false;
            _users.InsertOne(user);
            return Ok(user);
        }

        // Models
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}