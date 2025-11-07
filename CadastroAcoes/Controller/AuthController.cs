using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Model;
using Model.Repository;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

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
            if (await _repo.GetByApelidoAsync(registerDto.Apelido) != null)
                return BadRequest("Este apelido já está em uso.");

            if (await _repo.GetByEmailAsync(registerDto.Email) != null)
                return BadRequest("Este email já está em uso.");

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)));

            var user = new User
            {
                Username = registerDto.Username, // Mantendo Username se ainda for necessário
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
            // Admin login for development
            if (loginDto.Identifier == CadastroAcoes.TempDev.AdminConfig.Username && loginDto.Password == CadastroAcoes.TempDev.AdminConfig.Password)
            {
                var adminClaims = new[]
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: adminClaims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            var user = await _repo.GetByLoginIdentifierAsync(loginDto.Identifier);
            if (user == null) return Unauthorized();

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)));
            if (user.PasswordHash != hash) return Unauthorized();

            var token = GenerateToken(user.Apelido);
            return Ok(new { token });
        }

        private string GenerateToken(string apelido)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, apelido)
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
        public string Identifier { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string Username { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}