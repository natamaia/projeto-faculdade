using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Repository;

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

        [HttpPost]
        public async Task<IActionResult> CreateClientProfile([FromBody] CreateClientProfileDto dto)
        {
            if (dto.Cfp != 0 && await _repo.GetByCfpAsync(dto.Cfp) != null)
                return BadRequest("CPF já cadastrado");

            var userClient = new UserClient
            {
                UserId = dto.UserId,
                Cfp = dto.Cfp,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Cep = dto.Cep
            };

            await _repo.CreateAsync(userClient);
            return Ok(userClient);
        }
    }

    public class CreateClientProfileDto
    {
        public string UserId { get; set; } = null!;
        public long Cfp { get; set; }
        public string Address { get; set; } = null!;
        public long PhoneNumber { get; set; }
        public long Cep { get; set; }
    }
}
