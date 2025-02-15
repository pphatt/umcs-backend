using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Wrapper;

namespace Server.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<ErrorOr<ResponseWrapper>>
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

    public IFormFile? Avatar { get; set; } = default!;
}
