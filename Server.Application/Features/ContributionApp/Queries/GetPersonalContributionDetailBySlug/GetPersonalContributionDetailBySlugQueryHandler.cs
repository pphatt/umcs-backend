using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;

public class GetPersonalContributionDetailBySlugQueryHandler : IRequestHandler<GetPersonalContributionDetailBySlugQuery, ErrorOr<ResponseWrapper<PublicContributionDetailsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPersonalContributionDetailBySlugQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PublicContributionDetailsDto>>> Handle(GetPersonalContributionDetailBySlugQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ContributionRepository.GetPersonalContributionBySlug(request.Slug, request.UserId);

        return new ResponseWrapper<PublicContributionDetailsDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
