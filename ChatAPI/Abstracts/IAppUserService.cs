using ChatAPI.Models;

namespace ChatAPI.Abstracts
{
    public interface IAppUserService : IService<AppUser>
    {
        public Task<AppUser?> GetByUsername(string username);
    }
}
