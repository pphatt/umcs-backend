using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Wrapper;

namespace Server.Application.Features.FacultyApp.Queries.GetFacultyById;

public class GetFacultyByIdQuery : IRequest<ErrorOr<ResponseWrapper<FacultyDto>>>
{
    public Guid Id { get; set; }
}
