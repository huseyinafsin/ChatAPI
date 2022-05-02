using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatAPI.Common;
using ChatAPI.Data;
using ChatAPI.Jwt;
using ChatAPI.Models;
using Core.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using NuGet.Protocol;

namespace ChatAPI.Hubs
{
    public class AuthHub : Hub<ILoginHub>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ITokenHelper _tokenHelper;

        public AuthHub(UserManager<AppUser> userManager, RoleManager<IdentityRole<int>> roleManager, ITokenHelper tokenHelper)
        {
            try
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _tokenHelper = tokenHelper;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
       
        }


        public async Task Login(UserForLogin model)
        {
      

            var user = await _userManager.FindByNameAsync(model.UserName);

            AccessToken token = null;
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var roles = new List<string> {"Admin", "User"};

                token = _tokenHelper.CreateToken(user,roles);


            }

            await Clients.Caller.Login(token);
        }

        public async Task Register([FromBody]UserForRegister userForRegister)
        {
            AppUser user = new AppUser()
            {
                Email = userForRegister.Email,
                Firstname = userForRegister.Firstname,
                Lastname = userForRegister.Lastname,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userForRegister.Username,
            };
            var result = await _userManager.CreateAsync(user, userForRegister.Password);
            if (!result.Succeeded)
                await Clients.Caller.Create(false);

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.Admin.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole<int>(UserRoles.User.ToString()));


            if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
            }

            await Clients.Caller.Create(result.Succeeded);
        }
    }
}
