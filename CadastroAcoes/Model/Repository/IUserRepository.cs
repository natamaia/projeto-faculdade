using System.Threading.Tasks;
using Model;

namespace Model.Repository
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
    }
}
