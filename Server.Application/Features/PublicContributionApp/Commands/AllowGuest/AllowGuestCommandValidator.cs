using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuest;

public class AllowGuestCommandValidator : AbstractValidator<AllowGuestCommand>
{
    public AllowGuestCommandValidator()
    {
    }
}
