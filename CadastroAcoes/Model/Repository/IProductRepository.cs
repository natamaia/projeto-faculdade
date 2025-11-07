using System.Collections.Generic;
using System.Threading.Tasks;
using Model;

namespace Model.Repository
{
    public interface IProductRepository
    {
        Task CreateAsync(Product product);
        Task<Product?> GetByIdAsync(string id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByVendorAsync(string vendorId);
        Task UpdateAsync(Product product);
        Task DeleteAsync(string id);
    }
}
