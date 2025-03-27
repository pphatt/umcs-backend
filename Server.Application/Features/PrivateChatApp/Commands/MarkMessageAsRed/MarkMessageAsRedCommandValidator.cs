using FluentValidation;

using Server.Contracts.PrivateChats.MarkMessageAsRed;

namespace Server.Application.Features.PrivateChatApp.Commands.MarkMessageAsRed;

public class MarkMessageAsRedCommandValidator : AbstractValidator<MarkMessageAsRedRequest>
{
    public MarkMessageAsRedCommandValidator()
    {
    }
}
