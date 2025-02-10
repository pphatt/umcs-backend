using ErrorOr;
using MediatR;
using Server.Contracts.Identity.DeleteUser;

namespace Server.Application.Features.Identity.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<ErrorOr<DeleteUserResult>>
{
    public Guid Id { get; set; }
}
