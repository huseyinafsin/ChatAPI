using ChatAPI.Abstracts;
using ChatAPI.Models;

namespace ChatAPI.Concrete
{
    public class AppUserService : Service<AppUser>, IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;
        public AppUserService(IGenericRepository<AppUser> repository, IUnitOfWork unitOfWork, IAppUserRepository appUserRepository) : base(repository, unitOfWork)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<AppUser?> GetByUsername(string username)
        {
            return await _appUserRepository.GetByUsername(username);
        }
    }
}
