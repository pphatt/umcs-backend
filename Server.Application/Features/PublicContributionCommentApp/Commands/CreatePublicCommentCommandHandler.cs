using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionCommentApp.Commands;

public class CreatePublicCommentCommandHandler : IRequestHandler<CreatePublicCommentCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public CreatePublicCommentCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreatePublicCommentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Guest) && !contribution.AllowedGuest)
        {
            return Errors.Contribution.NotAllowYet;
        }

        _unitOfWork.ContributionPublicCommentRepository.Add(new ContributionPublicComment
        {
            Content = request.Content,
            ContributionId = request.ContributionId,
            UserId = request.UserId
        });

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Comment successfully."
        };
    }
}
