using ChatAPI.Models;
using Core.Dtos;

namespace ChatAPI.Jwt
{
public interface ITokenHelper
{
        AccessToken CreateToken(AppUser ortak,List<string> userRoles);
}
}



