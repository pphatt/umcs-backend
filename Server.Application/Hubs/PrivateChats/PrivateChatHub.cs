using System.Collections.Concurrent;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Entity.Identity;

namespace Server.Application.Hubs.PrivateChats;

[Authorize]
public class PrivateChatHub : Hub
{
    private readonly IUserService _userService;
    private readonly UserManager<AppUser> _userManager;

    public PrivateChatHub(IUserService userService, UserManager<AppUser> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        var currentUserId = _userService.GetUserId().ToString();

        if (!string.IsNullOrEmpty(currentUserId))
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            if (currentUser != null)
            {
                HubConnection.AddUserConnection(currentUserId, Context.ConnectionId);

                if (!currentUser.IsOnline)
                {
                    currentUser.IsOnline = true;
                    await _userManager.UpdateAsync(currentUser);

                    await Clients.Users(HubConnection.GetAllOnlineUsers()).SendAsync("NewUserConnected", new NewUserConnectedDto
                    {
                        UserId = currentUserId,
                        Username = currentUser.UserName,
                    });
                }
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var currentUserId = _userService.GetUserId().ToString();

        if (!string.IsNullOrEmpty(currentUserId))
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            var connections = HubConnection.GetUserConnections(currentUserId);

            if (currentUser != null && connections is not null)
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                }

                if (connections.Count == 0)
                {
                    HubConnection._userConnections.TryRemove(currentUserId, out _);

                    if (currentUser.IsOnline)
                    {
                        currentUser.IsOnline = false;
                        await _userManager.UpdateAsync(currentUser);

                        await Clients.Users(HubConnection.GetAllOnlineUsers()).SendAsync("NewUserDisconnected", new NewUserDisconnectedDto
                        {
                            UserId = currentUserId,
                            Username = currentUser.UserName,
                        });
                    }
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Ping", "Ok");
    }

    public async Task IsTyping(string receiverId)
    {
        var userId = _userService.GetUserId().ToString();
        var currentUser = await _userManager.FindByIdAsync(userId);

        var receiverConnections = HubConnection.GetUserConnections(receiverId);

        if (receiverConnections is not null)
        {
            await Clients.Clients(receiverConnections.ToList()).SendAsync("ReceiverIsTyping", true, currentUser?.Avatar);
        }
    }

    public async Task StopTyping(string receiverId)
    {
        var receiverConnections = HubConnection.GetUserConnections(receiverId);

        if (receiverConnections is not null)
        {
            await Clients.Clients(receiverConnections.ToList()).SendAsync("ReceiverStopTyping", false);
        }
    }
}
