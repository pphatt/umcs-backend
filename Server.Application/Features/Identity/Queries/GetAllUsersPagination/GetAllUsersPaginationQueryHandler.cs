using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Queries.GetAllUsersPagination;

public class GetAllUsersPaginationQueryHandler : IRequestHandler<GetAllUsersPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<UserDto>>>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersPaginationQueryHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<UserDto>>>> Handle(GetAllUsersPaginationQuery request, CancellationToken cancellationToken)
    {
        var allUserQuery = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            allUserQuery = allUserQuery.Where(
                user => user.Email!.Contains(request.Keyword) ||
                        user.UserName!.Contains(request.Keyword) ||
                        user.FirstName!.Contains(request.Keyword) ||
                        user.LastName!.Contains(request.Keyword) ||
                        user.PhoneNumber!.Contains(request.Keyword)
            );
        }

        if (request.RoleName is not null)
        {
            var allUsersInRole = (await _userManager.GetUsersInRoleAsync(request.RoleName)).Select(x => x.Id);

            if (allUsersInRole.Count() > 0)
            {
                allUserQuery = allUserQuery.Where(x => allUsersInRole.Contains(x.Id));
            }
        }

        if (request.FacultyName is not null)
        {
            var faculty = await _unitOfWork.FacultyRepository.GetFacultyByNameAsync(request.FacultyName);

            if (faculty is not null)
            {
                allUserQuery = allUserQuery.Where(x => x.FacultyId == faculty.Id);
            }
        }

        var count = await allUserQuery.CountAsync();

        var pageIndex = request.PageIndex < 0 ? 1 : request.PageIndex;
        var skipPages = (pageIndex - 1) * request.PageSize;

        allUserQuery =
            allUserQuery
                .OrderBy(x => x.DateCreated)
                .Skip(skipPages)
                .Take(request.PageSize);

        var result = await _mapper.ProjectTo<UserDto>(allUserQuery).ToListAsync(cancellationToken);

        foreach (var userDto in result)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user is null)
            {
                continue;
            }

            var roles = await _userManager.GetRolesAsync(user);
            userDto.Roles = roles;

            if (user.FacultyId is not null)
            {
                var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(user.FacultyId.Value);

                userDto.Faculty = faculty.Name ?? null;
            }
        }

        return new ResponseWrapper<PaginationResult<UserDto>>
        {
            IsSuccessful = true,
            ResponseData = new PaginationResult<UserDto>
            {
                CurrentPage = request.PageIndex,
                PageSize = request.PageSize,
                Results = result,
                RowCount = count
            }
        };
    }
}
