using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Commands.UpdateRole;

public class UpdateRoleCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; } = default!;

    public string Name { get; set; } = default!;
}
