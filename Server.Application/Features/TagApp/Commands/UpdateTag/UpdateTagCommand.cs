using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.TagApp.Commands.UpdateTag;

public class UpdateTagCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }

    public string TagName { get; set; } = default!;
}
