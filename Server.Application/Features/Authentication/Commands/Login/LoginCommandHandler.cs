using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Contracts.Authentication.Login;
using Server.Domain.Entity.Identity;
using Server.Domain.Entity.Token;

namespace Server.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    ILogger<LoginCommandHandler> _logger;
    UserManager<AppUser> _userManager;
    ITokenService _tokenService;
    IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(ILogger<LoginCommandHandler> logger, UserManager<AppUser> userManager, ITokenService tokenService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _logger = logger;
        _userManager = userManager;
        _tokenService = tokenService;
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
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _tokenService.StoreRefreshTokenAsync(user.Id, refreshToken);

        return new LoginResult(accessToken, refreshToken);
    }
}
