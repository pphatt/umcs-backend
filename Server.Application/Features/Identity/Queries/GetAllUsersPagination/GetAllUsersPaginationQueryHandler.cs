using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Enums;
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
        var query = _userManager.Users;

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(
                user => user.Email!.Contains(request.Keyword) ||
                        user.UserName!.Contains(request.Keyword) ||
                        user.FirstName!.Contains(request.Keyword) ||
                        user.LastName!.Contains(request.Keyword) ||
                        user.PhoneNumber!.Contains(request.Keyword)
            );
        }

        var roleFlag = false;

        if (!string.IsNullOrWhiteSpace(request.RoleName))
        {
            var allUsersInRole = (await _userManager.GetUsersInRoleAsync(request.RoleName)).Select(x => x.Id);

            if (allUsersInRole.Count() > 0)
            {
                query = query.Where(x => allUsersInRole.Contains(x.Id));

                roleFlag = true;
            }
        }

        var facultyName = string.Empty;

        if (!string.IsNullOrWhiteSpace(request.FacultyName))
        {
            var faculty = await _unitOfWork.FacultyRepository.GetFacultyByNameAsync(request.FacultyName);

            if (faculty is not null)
            {
                query = query.Where(x => x.FacultyId == faculty.Id);

                facultyName = faculty.Name;
            }
        }

        var isAscending = !string.IsNullOrWhiteSpace(request.OrderBy) &&
                          Enum.TryParse<OrderByEnum>(request.OrderBy, true, out var enumOrderBy) &&
                          enumOrderBy == OrderByEnum.Ascending;

        if (isAscending)
        {
            query = query.OrderBy(x => x.DateCreated);
        }
        else
        {
            query = query.OrderByDescending(x => x.DateCreated);
        }

        var count = await query.CountAsync();

        var pageIndex = request.PageIndex < 0 ? 1 : request.PageIndex;
        var skipPages = (pageIndex - 1) * request.PageSize;

        query = query
            .Skip(skipPages)
            .Take(request.PageSize);

        var result = await _mapper.ProjectTo<UserDto>(query).ToListAsync(cancellationToken);

        foreach (var userDto in result)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user is null)
            {
                continue;
            }

            if (!roleFlag)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles;
            }
            else
            {
                userDto.Roles = new List<string> { request.RoleName! };
            }

            if (!string.IsNullOrEmpty(facultyName))
            {
                userDto.Faculty = facultyName;
                continue;
            }

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
