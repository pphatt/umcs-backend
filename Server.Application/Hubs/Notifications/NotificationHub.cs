using Microsoft.AspNetCore.SignalR;

namespace Server.Application.Hubs.Notifications;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the hub.");
        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} says: {message}");
    }
}
