using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Commands.CreateRole;

public class CreateRoleCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string DisplayName { get; set; } = default!;

    public string Name { get; set; } = default!;
}
