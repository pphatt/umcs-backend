using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Common.Interfaces.Persistence;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ErrorOr<UserDto>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdQueryHandler(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        if (!user.IsActive)
        {
            return Errors.User.InactiveOrLockedOut;
        }

        var result = _mapper.Map<UserDto>(user);

        if (user.FacultyId is not null)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(user.FacultyId.Value);
            result.Faculty = faculty.Name ?? null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        
        if (roles is not null) 
        {
            result.Roles = roles;
        }

        return result;
    }
}
