using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionApp.Commands.ApproveContribution;

public class ApproveContributionCommandHandler : IRequestHandler<ApproveContributionCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveContributionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ApproveContributionCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.ContributionRepository.ApproveContribution(request.Id);

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Approve project successfully."
        };
    }
}
