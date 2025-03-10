using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.TagApp.Commands.UpdateTag;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateTagCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
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

        var alreadyExist = await _unitOfWork.TagRepository.GetTagByName(request.TagName);

        if (alreadyExist is not null)
        {
            return Errors.Tags.AlreadyExist;
        }

        tag.Name = request.TagName;
        tag.DateUpdated = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Update tag's name successfully."
        };
    }
}
