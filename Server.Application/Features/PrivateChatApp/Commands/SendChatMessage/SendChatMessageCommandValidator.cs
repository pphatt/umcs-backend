using FluentValidation;

namespace Server.Application.Features.PrivateChatApp.Commands.SendChatMessage;

public class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
{
    public SendChatMessageCommandValidator()
    {
    }
}
