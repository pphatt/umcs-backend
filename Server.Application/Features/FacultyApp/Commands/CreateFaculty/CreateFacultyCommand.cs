using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.FacultyApp.Commands.CreateFaculty;

public class CreateFacultyCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string Name { get; set; } = default!;
}
