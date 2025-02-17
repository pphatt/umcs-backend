using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.FacultyApp.Commands.DeleteFaculty;

public class DeleteFacultyCommandHandler : IRequestHandler<DeleteFacultyCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UserManager<AppUser> _userManager;

    public DeleteFacultyCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteFacultyCommand request, CancellationToken cancellationToken)
    {
        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.Id);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        var hasUserIn = await _userManager.Users.FirstOrDefaultAsync(x => x.FacultyId == faculty.Id);

        if (hasUserIn is not null)
        {
            return Errors.Faculty.HasUserIn;
        }

        faculty.DateDeleted = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete faculty successfully."
        };
    }
}
