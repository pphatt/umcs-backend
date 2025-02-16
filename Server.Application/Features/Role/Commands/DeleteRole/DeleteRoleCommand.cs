using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
