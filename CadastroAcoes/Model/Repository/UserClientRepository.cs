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

        public async Task<UserClient?> GetByCfpAsync(int cfp) =>
            await _clients.Find(u => u.Cfp == cfp).FirstOrDefaultAsync();
    }
}
