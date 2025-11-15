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
                return BadRequest(new ErrorResponse { Code = "INVALID_NICKNAME", Message = "Este apelido já está em uso ou é inválido." });

            if (registerDto.Email == null || await _repo.GetByEmailAsync(registerDto.Email) != null)
                return BadRequest(new ErrorResponse { Code = "INVALID_EMAIL", Message = "Este email já está em uso ou é inválido." });

            if (string.IsNullOrEmpty(registerDto.Password))
                return BadRequest(new ErrorResponse { Code = "PASSWORD_REQUIRED", Message = "A senha é obrigatória." });

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)));

            var user = new User
            {
                Username = registerDto.Username ?? string.Empty,
                Apelido = registerDto.Apelido,
                Email = registerDto.Email,
                PasswordHash = hash
            };

            try
            {
                await _repo.CreateAsync(user);
                return Ok(user);
            }
            catch (MongoException mex)
            {
                return StatusCode(503, new ErrorResponse { Code = "DB_UNAVAILABLE", Message = "Serviço de banco de dados indisponível.", Details = mex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { Code = "INTERNAL_ERROR", Message = "Erro ao criar usuário.", Details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Identifier) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new ErrorResponse { Code = "INVALID_PAYLOAD", Message = "Identifier and Password are required." });
            }

            User? user;
            try
            {
                user = await _repo.GetByLoginIdentifierAsync(loginDto.Identifier);
            }
            catch (MongoException ex)
            {
                return StatusCode(503, new ErrorResponse { Code = "DB_UNAVAILABLE", Message = "Serviço de banco de dados indisponível.", Details = ex.Message });
            }
            catch (TimeoutException ex)
            {
                return StatusCode(503, new ErrorResponse { Code = "DB_TIMEOUT", Message = "Tempo de conexão com o banco expirou.", Details = ex.Message });
            }

            if (user == null) return Unauthorized(new ErrorResponse { Code = "INVALID_CREDENTIALS", Message = "Usuário ou senha inválidos." });

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));
            if (user.PasswordHash != hash) return Unauthorized(new ErrorResponse { Code = "INVALID_CREDENTIALS", Message = "Usuário ou senha inválidos." });

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
