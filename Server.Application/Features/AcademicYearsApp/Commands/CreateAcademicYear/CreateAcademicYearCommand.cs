using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;

public class CreateAcademicYearCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public required string Name { get; set; }

    public required DateTime StartClosureDate { get; set; }

    public required DateTime EndClosureDate { get; set; }

    public required DateTime FinalClosureDate { get; set; }

    public required bool IsActive { get; set; } = false;
}
