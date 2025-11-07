using MongoDB.Driver;
using Model;

namespace Model.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IMongoDatabase db)
        {
            _users = db.GetCollection<User>("Users");
        }

        public async Task CreateAsync(User user) =>
            await _users.InsertOneAsync(user);

        public async Task<User?> GetByApelidoAsync(string apelido) =>
            await _users.Find(u => u.Apelido == apelido).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<User?> GetByLoginIdentifierAsync(string identifier) =>
            await _users.Find(u => u.Apelido == identifier || u.Email == identifier).FirstOrDefaultAsync();
    }
}