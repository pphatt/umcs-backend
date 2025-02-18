using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;

public class DeleteAcademicYearCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
