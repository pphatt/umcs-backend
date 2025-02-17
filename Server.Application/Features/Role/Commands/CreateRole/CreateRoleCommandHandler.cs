using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ErrorOr<ResponseWrapper>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public CreateRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
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

        var createNewRole = new AppRole
        {
            DisplayName = request.DisplayName,
            Name = request.Name,
            NormalizedName = request.Name.ToUpperInvariant(),
        };

        var result = await _roleManager.CreateAsync(createNewRole);

        if (!result.Succeeded)
        {
            return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create new role successfully."
        };
    }
}
