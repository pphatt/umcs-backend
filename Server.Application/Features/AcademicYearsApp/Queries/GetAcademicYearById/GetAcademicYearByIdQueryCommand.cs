using AutoMapper;
using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.AcademicYearsApp.Queries.GetAcademicYearById;

public class GetAcademicYearByIdQueryCommand : IRequestHandler<GetAcademicYearByIdQuery, ErrorOr<ResponseWrapper<AcademicYearDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAcademicYearByIdQueryCommand(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<AcademicYearDto>>> Handle(GetAcademicYearByIdQuery request, CancellationToken cancellationToken)
    {
        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.Id);

        if (academicYear is null)
        {
            return Errors.AcademicYears.CannotFound;
        }

        var result = _mapper.Map<AcademicYearDto>(academicYear);

        return new ResponseWrapper<AcademicYearDto>
        {
            IsSuccessful = true,
            ResponseData = result,
        };
    }
}
