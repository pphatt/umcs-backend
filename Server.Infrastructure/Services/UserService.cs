using Microsoft.AspNetCore.Http;
using Server.Application.Common.Interfaces.Services;

namespace Server.Infrastructure.Services;

public class UserService : IUserService
{
    IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool? IsAuthenticated() 
        => _httpContextAccessor
            .HttpContext?
            .User
            .Identity?
            .IsAuthenticated;
}
