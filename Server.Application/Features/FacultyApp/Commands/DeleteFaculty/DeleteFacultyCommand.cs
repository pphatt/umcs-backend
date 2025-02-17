using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.FacultyApp.Commands.DeleteFaculty;

public class DeleteFacultyCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
