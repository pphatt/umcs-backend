using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;

public class InactivateAcademicYearCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
