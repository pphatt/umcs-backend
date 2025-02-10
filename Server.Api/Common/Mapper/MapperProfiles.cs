using AutoMapper;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Application.Features.Users.Commands.CreateUser;
using Server.Contracts.Authentication.Login;
using Server.Contracts.Authentication.RefreshToken;
using Server.Contracts.Identity.CreateUser;
using Server.Contracts.Identity.UpdateUser;
using Server.Domain.Entity.Identity;

namespace Server.Api.Common.Mapper;

public class MapperProfiles : Profile
{
    public MapperProfiles() 
    {
        // Authentication.
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();

        // Create User.
        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<CreateUserCommand, AppUser>();

        // Update User.
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UpdateUserCommand, AppUser>();
    }
}
