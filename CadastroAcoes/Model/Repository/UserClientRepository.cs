using MongoDB.Driver;
using Model;

namespace Model.Repository
{
    public class UserClientRepository : IUserClientRepository
    {
        private readonly IMongoCollection<UserClient> _clients;
        public UserClientRepository(IMongoDatabase db)
        {
            _clients = db.GetCollection<UserClient>("UserClients");
        }

        public async Task CreateAsync(UserClient user) =>
            await _clients.InsertOneAsync(user);

        public async Task<UserClient?> GetByUsernameAsync(string username) =>
            await _clients.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task<UserClient?> GetByEmailAsync(string email) =>
            await _clients.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<UserClient?> GetByCfpAsync(int cfp) =>
            await _clients.Find(u => u.Cfp == cfp).FirstOrDefaultAsync();
    }
}
