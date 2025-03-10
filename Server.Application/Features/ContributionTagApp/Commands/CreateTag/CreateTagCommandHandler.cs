using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.ContributionTagApp.Commands.CreateTag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.TagRepository.GetTagByName(request.TagName);

        if (tag is not null)
        {
            return Errors.Tags.AlreadyExist;
        }

        _unitOfWork.TagRepository.Add(new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.TagName
        });

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create new tag successfully."
        };
    }
}
