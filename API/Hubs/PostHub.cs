using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostHub : Hub<IPostHub>
    {
        public override Task OnConnectedAsync()
        {
            Console.Write(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        } 
        public async Task post(string messege)
        { 
            await Clients.All.NotfiyPostadded(messege);
        }

    }
    public interface IPostHub
    {
        public Task NotfiyPostadded(string posttext);
    }
}