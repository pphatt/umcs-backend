using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Queries.GetRoleById;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, ErrorOr<ResponseWrapper<RoleDto>>>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;

    public GetRoleByIdQueryHandler(RoleManager<AppRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<RoleDto>>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
        {
            return Errors.Roles.CannotFound;
        }

        var responseData = _mapper.Map<RoleDto>(role);

        return new ResponseWrapper<RoleDto>
        {
            IsSuccessful = true,
            ResponseData = responseData,
        };
    }
}
