using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Queries.GetPublicContributionBySlug;

public class GetPublicContributionBySlugQueryHandler : IRequestHandler<GetPublicContributionBySlugQuery, ErrorOr<ResponseWrapper<PublicContributionWithCommentsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public GetPublicContributionBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<PublicContributionWithCommentsDto>>> Handle(GetPublicContributionBySlugQuery request, CancellationToken cancellationToken)
    {
        var publicContributionDto = await _unitOfWork.ContributionPublicRepository.GetPublicContributionBySlug(request.Slug);

        if (publicContributionDto is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Guest))
        {
            if (!publicContributionDto.AllowedGuest)
            {
                return Errors.Contribution.NotAllowed;
            }
        }

        var publicContribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(publicContributionDto.Id);

        publicContribution.Views += 1;
        publicContributionDto.View = publicContribution.Views;
        publicContributionDto.AlreadyLike = await _unitOfWork.LikeRepository.AlreadyLike(publicContributionDto.Id, user.Id);
        publicContributionDto.AlreadySaveReadLater = await _unitOfWork.ContributionPublicReadLaterRepository.AlreadySave(publicContributionDto.Id, user.Id);
        publicContributionDto.AlreadyBookmark = await _unitOfWork.ContributionPublicBookmarkRepository.AlreadyBookmark(publicContributionDto.Id, user.Id);

        await _unitOfWork.CompleteAsync();

        var comments = await _unitOfWork.ContributionPublicCommentRepository.GetCommentsByContributionId(publicContribution.Id);

        var result = _mapper.Map<PublicContributionWithCommentsDto>(publicContributionDto);

        result.Comments = comments;

        return new ResponseWrapper<PublicContributionWithCommentsDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
