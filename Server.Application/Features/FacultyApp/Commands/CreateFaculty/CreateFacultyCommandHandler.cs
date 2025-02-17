using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.FacultyApp.Commands.CreateFaculty;

public class CreateFacultyCommandHandler : IRequestHandler<CreateFacultyCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFacultyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Errors.Faculty.InvalidName;
        }

        var nameExists = _unitOfWork.FacultyRepository.GetFacultyByNameAsync(request.Name);

        if (nameExists is not null)
        {
            return Errors.Faculty.DuplicateName;
        }

        var faculty = new Faculty
        {
            Name = request.Name,
        };

        _unitOfWork.FacultyRepository.Add(faculty);

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create new faculty successfully."
        };
    }
}
