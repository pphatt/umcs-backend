using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Server.Application.Common.Interfaces.Persistence;
using Server.Contracts.Identity.UpdateUser;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<UpdateUserResult>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<UpdateUserResult>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // find user.
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var newRole = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (newRole is null) 
        {
            return Errors.Roles.NotFound;
        }

        var newFaculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId);

        if (newFaculty is null)
        {
            return Errors.Faculty.NotFound;
        }

        // remove current role.
        var currentRole = await _userManager.GetRolesAsync(user);
        var roleRemovalResult = await _userManager.RemoveFromRolesAsync(user, currentRole);

        if (!roleRemovalResult.Succeeded)
        {
            return roleRemovalResult.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        // add new role.
        var addedRoleResult = await _userManager.AddToRoleAsync(user, newRole.Name!);

        if (!addedRoleResult.Succeeded)
        {
            return addedRoleResult.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        // update the rest of the request.
        _mapper.Map(request, user);

        user.FacultyId = newFaculty.Id;

        // save updated user.
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        return new UpdateUserResult(Message: "Update user successfully.");
    }
}
