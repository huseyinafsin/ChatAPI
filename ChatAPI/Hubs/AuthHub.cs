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
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenHelper = tokenHelper;
        }


        public async Task Login([FromBody]UserForLogin model)
        {
      

            var user = await _userManager.FindByNameAsync(model.UserName);

            AccessToken token = null;
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var accessToken = _tokenHelper.CreateToken(user,userRoles);


            }
            await Clients.Caller.Login(user != null ? token : null);
        }

        public async Task Create([FromBody]UserForRegister userForRegister)
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
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));


            if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
            }

            await Clients.Caller.Create(result.Succeeded);
        }
    }
}
