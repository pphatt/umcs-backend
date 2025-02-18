using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;

public class BulkDeleteFacultiesCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> FacultyIds { get; set; } = default!;
}
