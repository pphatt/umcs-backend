using FluentValidation;

namespace Server.Application.Features.ContributionApp.Commands.CreateContribution;

public class CreateContributionCommandValidator : AbstractValidator<CreateContributionCommand>
{
    public CreateContributionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull()
            .NotEmpty()
            .WithMessage("Title must not be empty.")
            .MaximumLength(256)
            .WithMessage("Title's length must be less than 256.");

        RuleFor(x => x.FacultyId)
            .NotEmpty()
            .WithMessage("Faculty ID must not be empty.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID must not be empty.");

        RuleFor(x => x.IsConfirmed)
            .NotNull()
            .NotEmpty()
            .WithMessage("Please accept term and condition before submitting.");
    }
}
