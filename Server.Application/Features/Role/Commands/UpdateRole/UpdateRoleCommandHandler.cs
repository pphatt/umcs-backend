using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ErrorOr<ResponseWrapper>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public UpdateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleExists = await _roleManager.FindByNameAsync(request.Name);

        if (roleExists is not null)
        {
            return Errors.Roles.NameDuplicated;
        }

        var roleDisplayNameExists = _roleManager.Roles.Where(r => r.DisplayName == request.DisplayName).FirstOrDefault();

        if (roleDisplayNameExists is not null)
        {
            return Errors.Roles.DisplayNameDuplicated;
        }

        if (string.IsNullOrWhiteSpace(request.Id.ToString()))
        {
            return Errors.Roles.EmptyId;
        }

        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
        {
            return Errors.Roles.CannotFound;
        }

        role.DisplayName = request.DisplayName;
        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Update role successfully."
        };
    }
}
