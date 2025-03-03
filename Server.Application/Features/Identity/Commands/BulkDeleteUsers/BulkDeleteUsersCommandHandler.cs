using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.BulkDeleteUsers;

public class BulkDeleteUsersCommandHandler : IRequestHandler<BulkDeleteUsersCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public BulkDeleteUsersCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(BulkDeleteUsersCommand request, CancellationToken cancellationToken)
    {
        var usersIds = request.UserIds;
        var successfullyDeletedItems = new List<Guid>();

        foreach (var userId in usersIds)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Errors.User.CannotFound;
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any())
            {
                return Errors.Roles.CannotFound;
            }

            if (roles.Contains(Roles.Admin))
            {
                return Errors.User.CannotDelete;
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

            successfullyDeletedItems.Add(userId);
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = $"Successfully deleted {successfullyDeletedItems.Count()} users."
        };
    }
}
