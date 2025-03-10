using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.TagApp.Commands.BulkDeleteTags;

public class BulkDeleteTagsCommandHandler : IRequestHandler<BulkDeleteTagsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BulkDeleteTagsCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(BulkDeleteTagsCommand request, CancellationToken cancellationToken)
    {
        var tagsIds = request.TagIds;
        var successfullyDeletedItems = new List<Guid>();

        foreach (var id in request.TagIds)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);

            if (tag is null)
            {
                return Errors.Tags.CannotFound;
            }

            if (tag.DateDeleted.HasValue)
            {
                return Errors.Tags.AlreadyDeleted;
            }

            tag.DateDeleted = _dateTimeProvider.UtcNow;

            successfullyDeletedItems.Add(tag.Id);
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Messages = new List<string>
            {
                $"Successfully deleted {successfullyDeletedItems.Count} tags.",
                "Each item is available for recovery."
            }
        };
    }
}
