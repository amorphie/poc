using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using amorphie.poc.signalr_server.Model;
using Microsoft.AspNetCore.SignalR;

namespace amorphie.poc.signalr_server.Hubs;

    public class PrivateMessageHub : Hub
{
    public async Task SendMessage(HubMessage message)
    {

    }
    /*
    public async Task SendMessageToClient(HubMessage hubMessage) =>
        await Clients.Client(connectionId).SendAsync("SendMessage", data);

    public async Task SendMessageToUser(string data, string userId) =>
        await Clients.User(userId).SendAsync("SendMessage", data);
*/

}


