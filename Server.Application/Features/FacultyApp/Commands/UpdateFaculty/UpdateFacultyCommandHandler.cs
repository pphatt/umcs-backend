using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.FacultyApp.Commands.UpdateFaculty;

public class UpdateFacultyCommandHandler : IRequestHandler<UpdateFacultyCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateFacultyCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UpdateFacultyCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Errors.Faculty.InvalidName;
        }

        var nameExists = _unitOfWork.FacultyRepository.FindByCondition(x => x.Name == request.Name).FirstOrDefault();

        if (nameExists is not null)
        {
            return Errors.Faculty.DuplicateName;
        }

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.Id);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        faculty.Name = request.Name;
        faculty.DateUpdated = _dateTimeProvider.UtcNow;

        _unitOfWork.FacultyRepository.Update(faculty);

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Update faculty successfully."
        };
    }
}
