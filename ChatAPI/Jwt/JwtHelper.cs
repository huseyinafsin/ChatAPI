using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatAPI.Models;
using Core.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace ChatAPI.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private DateTime _accessTokenExpiration;
        private JwtSecurityToken _jwtSecurityToken;

        public AccessToken CreateToken(AppUser user, List<string> userRoles)
        {
            _accessTokenExpiration = DateTime.Now.AddHours(1);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MOGQodmQdS1DoPoSMkjB2tq4A7gr2ZMCmFso5swounToBXZCXfmXk6FdPvaHQ2l3"));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            _jwtSecurityToken = new JwtSecurityToken(
                issuer: "https://github.com/huseyinafsin",
                audience: "https://github.com/huseyinafsin",
                expires: DateTime.Now.AddDays(1),
                notBefore: DateTime.Now,
                claims: SetClaims(user,userRoles),
                signingCredentials: signingCredentials


            );
            var token = jwtSecurityTokenHandler.WriteToken(_jwtSecurityToken);


            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration
            };

        }

        private IEnumerable<Claim> SetClaims(AppUser user,List<string> userRoles)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Firstname+" "+user.Lastname),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                

            };
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }


            return claims;
        }
    }

}
