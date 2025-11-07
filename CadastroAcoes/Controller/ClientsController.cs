using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Repository;
using System.Security.Cryptography;
using System.Text;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IUserClientRepository _repo;

        public ClientsController(IUserClientRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserClient dto)
        {
            if (await _repo.GetByUsernameAsync(dto.Username) != null)
                return BadRequest("Usuário já existe");
            if (await _repo.GetByEmailAsync(dto.Email) != null)
                return BadRequest("Email já cadastrado");
            if (await _repo.GetByCfpAsync(dto.Cfp) != null)
                return BadRequest("CPF já cadastrado");

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(dto.PasswordHash)));

            var user = new UserClient
            {
                Username = dto.Username,
                Cfp = dto.Cfp,
                Email = dto.Email,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Cep = dto.Cep,
                PasswordHash = hash
            };

            await _repo.CreateAsync(user);
            return Ok();
        }
    }
}
