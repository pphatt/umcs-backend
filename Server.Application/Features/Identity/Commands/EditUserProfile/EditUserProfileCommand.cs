using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.EditUserProfile;

public class EditUserProfileCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? Dob { get; set; }

    public string? PhoneNumber { get; set; }

    public IFormFile? Avatar { get; set; }
}
