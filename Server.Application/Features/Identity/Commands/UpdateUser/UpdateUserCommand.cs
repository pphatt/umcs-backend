using ErrorOr;
using MediatR;
using Server.Contracts.Identity.UpdateUser;

namespace Server.Application.Features.Identity.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<ErrorOr<UpdateUserResult>>
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid FacultyId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? Dob { get; set; }

    public bool IsActive { get; set; }
}
