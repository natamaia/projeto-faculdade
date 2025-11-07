using MongoDB.Driver;
using Model;

namespace Model.Repository
{
    public class UserVendorRepository : IUserVendorRepository
    {
        private readonly IMongoCollection<UserVendor> _vendors;
        public UserVendorRepository(IMongoDatabase db)
        {
            _vendors = db.GetCollection<UserVendor>("UserVendors");
        }

        public async Task CreateAsync(UserVendor user) =>
            await _vendors.InsertOneAsync(user);

        public async Task<UserVendor?> GetByCnpjAsync(int cnpj) =>
            await _vendors.Find(u => u.cnpj == cnpj).FirstOrDefaultAsync();
    }
}
