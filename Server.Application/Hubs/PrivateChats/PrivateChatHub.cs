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
                if (!currentUser.IsOnline)
                {
                    currentUser.IsOnline = true;
                    await _userManager.UpdateAsync(currentUser);

                    // Notify all clients of the new online user
                    await Clients.All.SendAsync("NewUserConnected", new NewUserConnectedDto
                    {
                        UserId = currentUserId,
                        Username = currentUser.UserName,
                    });
                }

                // Add user to a group for their own ID (for targeting messages)
                await Groups.AddToGroupAsync(Context.ConnectionId, currentUserId);
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
            if (currentUser != null)
            {
                // Remove the connection from the user's group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentUserId);

                // Check if the user has any remaining connections
                var connections = Clients.Group(currentUserId);
                var hasConnections = await Task.Run(() => connections != null); // Simplified check

                if (!hasConnections && currentUser.IsOnline)
                {
                    currentUser.IsOnline = false;
                    await _userManager.UpdateAsync(currentUser);

                    // Notify all clients of the disconnection
                    await Clients.All.SendAsync("NewUserDisconnected", new NewUserDisconnectedDto
                    {
                        UserId = currentUserId,
                        Username = currentUser.UserName,
                    });
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
        await Clients.Group(receiverId).SendAsync("ReceiverIsTyping", true, currentUser?.Avatar);
    }

    public async Task StopTyping(string receiverId)
    {
        await Clients.Group(receiverId).SendAsync("ReceiverStopTyping", false);
    }
}
