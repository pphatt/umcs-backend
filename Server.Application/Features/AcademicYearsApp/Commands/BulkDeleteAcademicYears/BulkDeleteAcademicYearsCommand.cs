using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;

public class BulkDeleteAcademicYearsCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> AcademicIds { get; set; } = default!;
}
