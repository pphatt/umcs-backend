using AutoMapper;
using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.FacultyApp.Queries.GetFacultyById;

public class GetFacultyByIdQueryHandler : IRequestHandler<GetFacultyByIdQuery, ErrorOr<ResponseWrapper<FacultyDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFacultyByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<FacultyDto>>> Handle(GetFacultyByIdQuery request, CancellationToken cancellationToken)
    {
        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.Id);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        var result = _mapper.Map<FacultyDto>(faculty);

        return new ResponseWrapper<FacultyDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
