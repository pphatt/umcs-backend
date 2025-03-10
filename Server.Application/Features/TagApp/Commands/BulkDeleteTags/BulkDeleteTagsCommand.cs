using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.TagApp.Commands.BulkDeleteTags;

public class BulkDeleteTagsCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> TagIds { get; set; }
}
