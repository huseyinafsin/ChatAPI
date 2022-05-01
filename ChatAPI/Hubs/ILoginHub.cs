using System.IdentityModel.Tokens.Jwt;
using Core.Dtos;
using NuGet.Common;

namespace ChatAPI.Hubs
{
    public interface ILoginHub
    {
        Task Login(string token);
        Task Create(bool result);
    }
}
