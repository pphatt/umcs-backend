using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
