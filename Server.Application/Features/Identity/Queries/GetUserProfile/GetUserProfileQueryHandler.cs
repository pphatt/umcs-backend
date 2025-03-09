using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ErrorOr<ResponseWrapper<UserProfileDto>>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetUserProfileQueryHandler(UserManager<AppUser> userManager, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<UserProfileDto>>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        if (!user.IsActive)
        {
            return Errors.User.InactiveOrLockedOut;
        }

        var result = _mapper.Map<UserProfileDto>(user);

        if (user.FacultyId is not null)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(user.FacultyId.Value);
            result.Faculty = faculty.Name ?? null;
        }

        return new ResponseWrapper<UserProfileDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
