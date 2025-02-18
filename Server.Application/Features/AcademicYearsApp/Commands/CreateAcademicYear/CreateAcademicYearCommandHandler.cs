using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandHandler : IRequestHandler<CreateAcademicYearCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public CreateAcademicYearCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Errors.AcademicYears.InvalidName;
        }

        var nameExists = await _unitOfWork.AcademicYearRepository.GetAcademicYearByNameAsync(request.Name);

        if (nameExists is not null)
        {
            return Errors.AcademicYears.DuplicateName;
        }

        var userId = _userService.GetUserId();

        var academicYear = new AcademicYear
        {
            Name = request.Name,
            StartClosureDate = request.StartClosureDate,
            EndClosureDate = request.EndClosureDate,
            FinalClosureDate = request.FinalClosureDate,
            IsActive = true,
            UserIdCreated = userId,
        };

        _unitOfWork.AcademicYearRepository.Add(academicYear);

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create new academic year successfully."
        };
    }
}
