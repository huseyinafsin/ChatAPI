using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Hubs
{
    // [Authorize]
    // [Authorize(AuthenticationSchemes = Startup.CustomCookieScheme)]
    [Authorize(AuthenticationSchemes = Static.CustomCookieScheme + "," + Static.CustomTokenScheme)]
    public class ProtectedHub : Hub
    {
        [Authorize("Cookie")]
        public object CookieProtected()
        {
            var req = Context.GetHttpContext().Request;

            return CompileResult();
        }

        [Authorize("Token")]
        public object TokenProtected()
        {
            var req = Context.GetHttpContext().Request;

            return CompileResult();
        }

        private object CompileResult() =>
            new
            {
                UserId = Context.UserIdentifier,
                Claims = Context.User.Claims.Select(x => new { x.Type, x.Value })
            };
    }
}
