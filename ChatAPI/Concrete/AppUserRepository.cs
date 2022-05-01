using ChatAPI.Abstracts;
using ChatAPI.Data;
using ChatAPI.Models;

namespace ChatAPI.Concrete
{
    public class AppUserRepository : GenericRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AppUser?> GetByUsername(string username)
        {
            return _context.AppUsers.SingleOrDefault(x => x.UserName == username);
          
        }
    }
}
