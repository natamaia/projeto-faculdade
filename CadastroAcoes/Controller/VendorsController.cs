using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Repository;
using System.Security.Cryptography;
using System.Text;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IUserVendorRepository _repo;

        public VendorsController(IUserVendorRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserVendor dto)
        {
            if (await _repo.GetByUsernameAsync(dto.Username) != null)
                return BadRequest("Usuário já existe");
            if (await _repo.GetByEmailAsync(dto.Email) != null)
                return BadRequest("Email já cadastrado");
            if (await _repo.GetByCnpjAsync(dto.cnpj) != null)
                return BadRequest("CNPJ já cadastrado");

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(dto.PasswordHash)));

            var user = new UserVendor
            {
                Username = dto.Username,
                cnpj = dto.cnpj,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Empresa = dto.Empresa,
                PasswordHash = hash
            };

            await _repo.CreateAsync(user);
            return Ok();
        }
    }
}
