using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculties;

public class BulkDeleteFacultiesCommandHandler : IRequestHandler<BulkDeleteFacultiesCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BulkDeleteFacultiesCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(BulkDeleteFacultiesCommand request, CancellationToken cancellationToken)
    {
        var facultiesIds = request.FacultyIds;
        var successfullyDeletedItems = new List<Guid>();

        foreach (var id in facultiesIds)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(id);

            if (faculty is null)
            {
                return Errors.Faculty.CannotFound;
            }

            var hasUserIn = await _userManager.Users.FirstOrDefaultAsync(x => x.FacultyId == id);

            if (hasUserIn is not null)
            {
                return Errors.Faculty.HasUserIn;
            }

            faculty.DateDeleted = _dateTimeProvider.UtcNow;

            successfullyDeletedItems.Add(id);
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Messages = new List<string>
            {
                $"Successfully deleted {successfullyDeletedItems.Count} faculties.",
                "Each item is available for recovery."
            }
        };
    }
}
