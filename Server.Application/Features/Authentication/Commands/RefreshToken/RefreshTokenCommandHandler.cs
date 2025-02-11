using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Contracts.Authentication.RefreshToken;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<ResponseWrapper<RefreshTokenResult>>>
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

    public async Task<ErrorOr<ResponseWrapper<RefreshTokenResult>>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = _tokenService.GetByTokenAsync(request.RefreshToken);

        if (token is null)
        {
            return Errors.RefreshToken.Invalid;
        }

        if (token.RefreshTokenExpiryTime <= _dateTimeProvider.UtcNow) 
        {
            return Errors.RefreshToken.Expired;
        }

        var userOfToken = await _userManager.FindByIdAsync(token.UserId.ToString());

        if (userOfToken is null)
        {
            return Errors.User.CannotFound;
        }

        var newAccessToken = await _jwtTokenGenerator.GenerateToken(userOfToken);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        token.Token = newRefreshToken;
        token.RefreshTokenExpiryTime = _dateTimeProvider.UtcNow.AddMinutes(60);

        await _tokenService.UpdateRefreshTokenAsync(token);

        return new ResponseWrapper<RefreshTokenResult> 
        {
            IsSuccessful = true,
            Message = "Refresh token successfully.",
            ResponseData = new RefreshTokenResult(newAccessToken, newRefreshToken)
        };
    }
}
