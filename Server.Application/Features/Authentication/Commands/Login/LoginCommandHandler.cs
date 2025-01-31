using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Server.Contracts.Authentication.Login;
using Server.Domain.Common.Interfaces.Authentication;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    ILogger<LoginCommandHandler> _logger;
    UserManager<AppUser> _userManager;
    IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(ILogger<LoginCommandHandler> logger, UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _logger = logger;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            _logger.LogInformation("User not found.");
            throw new Exception("Email or password is incorrect.");
        }

        if (user.LockoutEnabled)
        {
            throw new Exception("User is locked out.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            _logger.LogInformation("Password is incorrect.");
            throw new Exception("Email or password is incorrect.");
        }

        var accessToken = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResult(accessToken, "");
    }
}
