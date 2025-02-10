using ErrorOr;
using MediatR;
using Server.Contracts.Identity.CreateUser;
using System.ComponentModel.DataAnnotations;

namespace Server.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<ErrorOr<CreateUserResult>>
{
    public string Email { get; set; } = default!;

    public string Username { get; set; } = default!;

    //public string Password { get; set; } = default!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid FacultyId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? Dob { get; set; }

    public bool IsActive { get; set; }
}
