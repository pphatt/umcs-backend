using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.FacultyApp.Commands.UpdateFaculty;

public class UpdateFacultyCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
}
