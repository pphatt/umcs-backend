using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;

public class ActivateAcademicYearCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
