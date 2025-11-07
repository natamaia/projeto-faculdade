using System.Threading.Tasks;
using Model;

namespace Model.Repository
{
    /// <summary>
    /// Contrato para persistência de UserClient.
    /// </summary>
    public interface IUserClientRepository
    {
        Task CreateAsync(UserClient user);
        Task<UserClient?> GetByUsernameAsync(string username);
        Task<UserClient?> GetByEmailAsync(string email);
        Task<UserClient?> GetByCfpAsync(int cfp);
    }
}
