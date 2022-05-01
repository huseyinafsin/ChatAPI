using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatAPI.Abstracts;
using ChatAPI.Common;
using ChatAPI.Data;
using ChatAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserForLogin userForLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Enter username and/or password correctly !");
            }

            var user = await _userManager.FindByNameAsync(userForLogin.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, userForLogin.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MOGQodmQdS1DoPoSMkjB2tq4A7gr2ZMCmFso5swounToBXZCXfmXk6FdPvaHQ2l3"));

                var token = new JwtSecurityToken(
                    issuer: "https://github.com/huseyinafsin",
                    audience: "https://github.com/huseyinafsin",
                    expires: DateTime.Now.AddDays(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized("You don't have access");

        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] UserForRegister userForRegister)
        {
            var userExists = await _userManager.FindByNameAsync(userForRegister.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { Status = "Error", Message = "User already exists!" });

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
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));
            

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
            }

            return Ok(new ResponseMessage { Status = "Success", Message = "User created successfully!" });
        }
    }
}
