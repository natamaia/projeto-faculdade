using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Model;
using Model.Repository;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto.Apelido == null || await _repo.GetByApelidoAsync(registerDto.Apelido) != null)
                return BadRequest("Este apelido já está em uso ou é inválido.");

            if (registerDto.Email == null || await _repo.GetByEmailAsync(registerDto.Email) != null)
                return BadRequest("Este email já está em uso ou é inválido.");

            if (string.IsNullOrEmpty(registerDto.Password))
                return BadRequest("A senha é obrigatória.");

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)));

            var user = new User
            {
                Username = registerDto.Username ?? string.Empty,
                Apelido = registerDto.Apelido,
                Email = registerDto.Email,
                PasswordHash = hash
            };

            await _repo.CreateAsync(user);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Identifier) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Identifier and Password are required.");
            }

            User? user;
            try
            {
                user = await _repo.GetByLoginIdentifierAsync(loginDto.Identifier);
            }
            catch (MongoException ex)
            {
                // Log the exception details here if you have a logging framework
                return StatusCode(503, new { message = "Database is currently unavailable. Please try again later.", details = ex.Message });
            }
            catch (TimeoutException ex)
            {
                return StatusCode(503, new { message = "Database connection timed out. Please try again later.", details = ex.Message });
            }

            if (user == null) return Unauthorized();

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));
            if (user.PasswordHash != hash) return Unauthorized();

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        private string GenerateToken(User user)
        {
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured in appsettings.json");
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Apelido ?? string.Empty)
            };
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresMinutes")),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        [Required]
        public string? Identifier { get; set; }
        [Required]
        public string? Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Apelido { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
