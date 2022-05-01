using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("[action]")]

        public IActionResult SayHello()
        {
            return Ok("Hello");
        }
    }
}
