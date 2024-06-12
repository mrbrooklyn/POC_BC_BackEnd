using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POC_Bangchak.Data;
using POC_Bangchak.Models;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace POC_Bangchak.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;

        public AuthController(
            ApplicationDbContext context,
            IConfiguration configuration
        )
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.PasswordHash != request.Password)
            {
                return BadRequest("Invalid email or password.");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]); // _configuration["JWT:Secret"]
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddYears(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var accessTokenExpireDate = DateTime.UtcNow.AddMinutes(30); // Token expiration time
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpireDate = DateTime.UtcNow.AddMonths(6);

            var Token = new TokenModel()
            {
                AccessToken = tokenString,
                AccessToken_CreateDate = DateTime.UtcNow,
                AccessToken_ExpireDate = accessTokenExpireDate,
                RefreshToken = refreshToken,
                RefreshToken_CreateDate = DateTime.UtcNow,
                RefreshToken_ExpireDate = refreshTokenExpireDate
            };

            return Ok(Token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Username already exists.");
            }

            // Here, you should hash the password before saving it to the database.
            // For simplicity, let's assume the password provided in the request is plain text.
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Role = request.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
