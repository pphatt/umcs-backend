using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Wrapper;

namespace Server.Application.Features.TagApp.Queries.GetTagById;

public class GetTagByIdQuery : IRequest<ErrorOr<ResponseWrapper<TagDto>>>
{
    public Guid Id { get; set; }
}
