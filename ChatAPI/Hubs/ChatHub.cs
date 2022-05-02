using ChatAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IMessageHub>
    {
        public async Task SendMessage(string message)
        {
            await Clients.Others.ReceiveMessage(message);
        }
    }

}

