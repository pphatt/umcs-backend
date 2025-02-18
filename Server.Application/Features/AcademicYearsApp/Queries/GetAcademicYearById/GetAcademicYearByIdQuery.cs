using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Wrapper;

namespace Server.Application.Features.AcademicYearsApp.Queries.GetAcademicYearById;

public class GetAcademicYearByIdQuery : IRequest<ErrorOr<ResponseWrapper<AcademicYearDto>>>
{
    public Guid Id { get; set; }
}
