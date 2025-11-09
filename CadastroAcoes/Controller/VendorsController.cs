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
            if (dto.Cnpj != 0 && await _repo.GetByCnpjAsync(dto.Cnpj) != null)
                return BadRequest("CNPJ já cadastrado");

            var userVendor = new UserVendor
            {
                UserId = dto.UserId,
                Cnpj = dto.Cnpj,
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
        public long Cnpj { get; set; }
        public string Empresa { get; set; } = null!;
        public long PhoneNumber { get; set; }
    }
}
