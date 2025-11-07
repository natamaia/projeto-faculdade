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

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }
}