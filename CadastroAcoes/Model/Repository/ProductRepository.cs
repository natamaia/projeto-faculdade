using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model;

namespace Model.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        public ProductRepository(IMongoDatabase db)
        {
            _products = db.GetCollection<Product>("Products");
        }

        public async Task CreateAsync(Product product) =>
            await _products.InsertOneAsync(product);

        public async Task<Product?> GetByIdAsync(string id) =>
            await _products.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _products.Find(_ => true).ToListAsync();

        public async Task<IEnumerable<Product>> GetByVendorAsync(string vendorId) =>
            await _products.Find(p => p.VendorId == vendorId).ToListAsync();

        public async Task UpdateAsync(Product product) =>
            await _products.ReplaceOneAsync(p => p.Id == product.Id, product);

        public async Task DeleteAsync(string id) =>
            await _products.DeleteOneAsync(p => p.Id == id);
    }
}
