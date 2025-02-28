using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogsByContributionId;

public class GetContributionActivityLogsByContributionIdQueryHandler : IRequestHandler<GetContributionActivityLogsByContributionIdQuery, ErrorOr<ResponseWrapper<List<ContributionActivityLogDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetContributionActivityLogsByContributionIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<List<ContributionActivityLogDto>>>> Handle(GetContributionActivityLogsByContributionIdQuery request, CancellationToken cancellationToken)
    {
        var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(request.Id);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var activity = await _unitOfWork.ContributionActivityLogRepository.GetContributionActivityLogsByContribution(contribution);

        return new ResponseWrapper<List<ContributionActivityLogDto>>
        {
            IsSuccessful = true,
            ResponseData = activity
        };
    }
}
