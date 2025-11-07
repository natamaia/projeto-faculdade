using System.Threading.Tasks;
using Model;

namespace Model.Repository
{
    /// <summary>
    /// Contrato para persistência de UserVendor.
    /// </summary>
    public interface IUserVendorRepository
    {
        Task CreateAsync(UserVendor user);
        Task<UserVendor?> GetByUsernameAsync(string username);
        Task<UserVendor?> GetByEmailAsync(string email);
        Task<UserVendor?> GetByCnpjAsync(int cnpj);
    }
}
