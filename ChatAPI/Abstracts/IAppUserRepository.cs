
using ChatAPI.Models;

namespace ChatAPI.Abstracts
{
    public interface IAppUserRepository :IGenericRepository<AppUser>
    {
        public  Task<AppUser?> GetByUsername(string username);
    }
}
