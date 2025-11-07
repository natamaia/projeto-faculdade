using System.Threading.Tasks;
using Model;

namespace Model.Repository
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByApelidoAsync(string apelido);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByLoginIdentifierAsync(string identifier);
    }
}
