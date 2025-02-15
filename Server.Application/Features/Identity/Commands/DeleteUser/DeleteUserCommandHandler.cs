using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (!roles.Any())
        {
            return Errors.Roles.CannotFound;
        }

        var roleRemovalResult = await _userManager.RemoveFromRolesAsync(user, roles);

        if (!roleRemovalResult.Succeeded)
        {
            return roleRemovalResult.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        var token = _unitOfWork.TokenRepository.FindByCondition(x => x.UserId == user.Id);

        if (token is not null)
        {
            _unitOfWork.TokenRepository.RemoveRange(token);
            await _unitOfWork.CompleteAsync();
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete user successfully."
        };
    }
}
