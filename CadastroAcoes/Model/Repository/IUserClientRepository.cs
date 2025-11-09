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
        Task<UserClient?> GetByCfpAsync(long cfp);
    }
}
