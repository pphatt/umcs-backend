using ErrorOr;

using MediatR;

using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;

namespace Server.Application.Features.PrivateChatApp.Commands.MarkMessageAsRed;

public class MarkMessageAsRedCommandHandler : IRequestHandler<MarkMessageAsRedCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkMessageAsRedCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(MarkMessageAsRedCommand request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.PrivateChatMessageRepository.GetAllAsync();

        var messageList = query
            .Where(x => x.ChatRoomId == request.ChatId && x.ReceiverId == request.CurrentUserId)
            .ToList();

        foreach (var message in messageList)
        {
            message.HasRed = true;
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Mark as red successfully."
        };
    }
}
