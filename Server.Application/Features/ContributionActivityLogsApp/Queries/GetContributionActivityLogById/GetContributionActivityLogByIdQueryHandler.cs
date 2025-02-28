using AutoMapper;
using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogById;

public class GetContributionActivityLogByIdQueryHandler : IRequestHandler<GetContributionActivityLogByIdQuery, ErrorOr<ResponseWrapper<ContributionActivityLogDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContributionActivityLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<ContributionActivityLogDto>>> Handle(GetContributionActivityLogByIdQuery request, CancellationToken cancellationToken)
    {
        var activity = await _unitOfWork.ContributionActivityLogRepository.GetByIdAsync(request.Id);

        if (activity is null)
        {
            return Errors.ContributionActivity.CannotFound;
        }

        var result = _mapper.Map<ContributionActivityLogDto>(activity);

        result.FromStatus = activity.FromStatus.ToStringValue();
        result.ToStatus = activity.ToStatus.ToStringValue();

        return new ResponseWrapper<ContributionActivityLogDto>
        {
            IsSuccessful = true,
            ResponseData = result,
        };
    }
}
