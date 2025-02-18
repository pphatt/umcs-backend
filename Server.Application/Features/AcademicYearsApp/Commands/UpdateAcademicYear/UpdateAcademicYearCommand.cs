using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;

public class UpdateAcademicYearCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }

    public string AcademicYearName { get; set; } = default!;

    public required DateTime StartClosureDate { get; set; }

    public required DateTime EndClosureDate { get; set; }

    public required DateTime FinalClosureDate { get; set; }
}
