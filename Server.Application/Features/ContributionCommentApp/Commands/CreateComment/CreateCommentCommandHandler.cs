using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.ContributionCommentApp.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public CreateCommentCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        if (contribution.FacultyId != user.FacultyId)
        {
            return Errors.Contribution.NotBelongToFaculty;
        }

        if (contribution.Status != ContributionStatus.Pending)
        {
            return Errors.Contribution.CannotCommentOnContributionAlreadyGraded;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Guest))
        {
            return Errors.Contribution.NotPublicYet;
        }

        if (role.Contains(Roles.Student) && user.Id != contribution.UserId)
        {
            return Errors.Contribution.NotBelongTo;
        }

        _unitOfWork.ContributionCommentRepository.Add(new ContributionComment
        {
            Content = request.Content,
            ContributionId = request.ContributionId,
            UserId = request.UserId
        });

        if (role.Contains(Roles.Coordinator))
        {
            if (!contribution.IsCoordinatorCommented)
            {
                contribution.IsCoordinatorCommented = true;

                var owner = await _userManager.FindByIdAsync(contribution.UserId.ToString());

                await _emailService.SendEmailAsync(new MailRequest
                {
                    ToEmail = owner.Email,
                    Subject = "Coordinator comment.",
                    Body = "Coordinator have commented on your contribution"
                });
            }
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Comment successfully."
        };
    }
}
