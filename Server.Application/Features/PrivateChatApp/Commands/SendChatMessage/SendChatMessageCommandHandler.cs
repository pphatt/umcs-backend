
using ErrorOr;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Hubs.PrivateChats;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PrivateChatApp.Commands.SendChatMessage;

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IHubContext<PrivateChatHub> _chatHub;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SendChatMessageCommandHandler(
        IHubContext<PrivateChatHub> chatHub,
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        IDateTimeProvider dateTimeProvider)
    {
        _chatHub = chatHub;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _userManager.FindByIdAsync(request.SenderId.ToString());

        if (sender is null)
        {
            return Errors.User.CannotFound;
        }

        var receiver = await _userManager.FindByIdAsync(request.ReceiverId.ToString());

        if (receiver is null)
        {
            return Errors.User.CannotFound;
        }

        var conversation = _unitOfWork.PrivateChatRoomRepository.FindByCondition(x =>
            (x.User1Id == sender.Id && x.User2Id == receiver.Id) ||
            (x.User1Id == receiver.Id && x.User2Id == sender.Id)
        ).FirstOrDefault();

        if (conversation is null)
        {
            conversation = new PrivateChatRoom
            {
                User1Id = sender.Id,
                User2Id = receiver.Id,
                User2LastActivity = _dateTimeProvider.UtcNow,
            };

            _unitOfWork.PrivateChatRoomRepository.Add(conversation);

            await _unitOfWork.CompleteAsync();
        }

        var message = new PrivateChatMessage
        {
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            ChatRoomId = conversation.Id,
            Content = request.Content
        };

        _unitOfWork.PrivateChatMessageRepository.Add(message);

        await _unitOfWork.CompleteAsync();

        // Notify the user.
        var messageDto = new PrivateChatMessageDto
        {
            ChatRoomId = conversation.Id,
            SenderId = sender.Id,
            SenderAvatar = sender.Avatar,
            ReceiverId = receiver.Id,
            ReceiverAvatar = receiver.Avatar,
            Content = message.Content,
            DateCreated = message.DateCreated
        };

        await _chatHub
            .Clients
            .Users(new List<string> { sender.Id.ToString(), receiver.Id.ToString() })
            .SendAsync("ReceiveNewPrivateMessage", messageDto);

        return new ResponseWrapper
        {
            IsSuccessful = true,
             Message = "Send message successfully."
        };
    }
}
