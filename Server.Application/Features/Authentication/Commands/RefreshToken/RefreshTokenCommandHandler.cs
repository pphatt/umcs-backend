﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Services;
using Server.Contracts.Authentication.RefreshToken;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    ITokenService _tokenService;
    IJwtTokenGenerator _jwtTokenGenerator;
    UserManager<AppUser> _userManager;
    IDateTimeProvider _dateTimeProvider;

    public RefreshTokenCommandHandler(ITokenService tokenService, IJwtTokenGenerator jwtTokenGenerator, UserManager<AppUser> userManager, IDateTimeProvider dateTimeProvider)
    {
        _tokenService = tokenService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GetByTokenAsync(request.RefreshToken);

        if (token is null)
        {
            throw new Exception("Invalid token.");
        }

        if (token.RefreshTokenExpiryTime <= _dateTimeProvider.UtcNow) 
        {
            throw new Exception("Refresh token expired.");
        }

        var userOfToken = await _userManager.FindByIdAsync(token.UserId.ToString());

        if (userOfToken is null)
        {
            throw new Exception("User not found.");
        }

        var newAccessToken = _jwtTokenGenerator.GenerateToken(userOfToken);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        token.Token = newRefreshToken;
        token.RefreshTokenExpiryTime = _dateTimeProvider.UtcNow.AddMinutes(60);

        await _tokenService.UpdateRefreshTokenAsync(token);

        return new RefreshTokenResult(newAccessToken, newRefreshToken);
    }
}
