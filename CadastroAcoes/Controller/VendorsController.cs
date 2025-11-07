using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Repository;

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

        [HttpPost]
        public async Task<IActionResult> CreateVendorProfile([FromBody] CreateVendorProfileDto dto)
        {
            if (dto.cnpj != 0 && await _repo.GetByCnpjAsync(dto.cnpj) != null)
                return BadRequest("CNPJ já cadastrado");

            var userVendor = new UserVendor
            {
                UserId = dto.UserId,
                cnpj = dto.cnpj,
                Empresa = dto.Empresa,
                PhoneNumber = dto.PhoneNumber
            };

            await _repo.CreateAsync(userVendor);
            return Ok(userVendor);
        }
    }

    public class CreateVendorProfileDto
    {
        public string UserId { get; set; } = null!;
        public int cnpj { get; set; }
        public string Empresa { get; set; } = null!;
        public int PhoneNumber { get; set; }
    }
}
