using Microsoft.AspNetCore.Http;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Services;
using System.Security.Claims;

namespace Server.Infrastructure.Services;

public class UserService : IUserService
{
    IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
        => _httpContextAccessor
            .HttpContext!
            .User
            .GetUserId();

    public bool? IsAuthenticated()
        => _httpContextAccessor
            .HttpContext?
            .User
            .Identity?
            .IsAuthenticated;
}
