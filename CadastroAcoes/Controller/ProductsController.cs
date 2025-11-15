using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repo.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<IActionResult> GetByVendor(string vendorId)
        {
            var items = await _repo.GetByVendorAsync(vendorId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            // If the client didn't set VendorId, try to get from authenticated user's claims
            var claimVendor = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(product.VendorId) && !string.IsNullOrEmpty(claimVendor)) product.VendorId = claimVendor;

            await _repo.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Product product)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // ensure the caller is the vendor owner (if claim available)
            var claimVendor = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(claimVendor) && !string.IsNullOrEmpty(existing.VendorId) && existing.VendorId != claimVendor)
            {
                return Forbid();
            }

            product.Id = id;
            // preserve vendorid unless explicitly same
            if (string.IsNullOrEmpty(product.VendorId)) product.VendorId = existing.VendorId;

            await _repo.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var claimVendor = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(claimVendor) && !string.IsNullOrEmpty(existing.VendorId) && existing.VendorId != claimVendor)
            {
                return Forbid();
            }

            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}
