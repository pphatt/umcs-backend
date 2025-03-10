using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.TagApp.Commands.DeleteTag;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteTagCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.TagRepository.GetByIdAsync(request.Id);

        if (tag is null)
        {
            return Errors.Tags.CannotFound;
        }

        if (tag.DateDeleted.HasValue)
        {
            return Errors.Tags.AlreadyDeleted;
        }

        tag.DateDeleted = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete tag successfully."
        };
    }
}
