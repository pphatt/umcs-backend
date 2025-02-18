using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.BulkDeleteUsers;

public class BulkDeleteUsersCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> UserIds { get; set; } = default!;
}
