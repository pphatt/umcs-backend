using AutoMapper;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Application.Features.Identity.Commands.DeleteUser;
using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Application.Features.Identity.Queries.GetAllUsersPagination;
using Server.Application.Features.Identity.Queries.GetUserById;
using Server.Application.Features.Users.Commands.CreateUser;
using Server.Contracts.Authentication.Login;
using Server.Contracts.Authentication.RefreshToken;
using Server.Contracts.Identity.CreateUser;
using Server.Contracts.Identity.DeleteUser;
using Server.Contracts.Identity.GetAllUsersPagination;
using Server.Contracts.Identity.GetUserById;
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

        // User.
        CreateMap<AppUser, UserDto>().ReverseMap();

        // Create User.
        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<CreateUserCommand, AppUser>();

        // Update User.
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UpdateUserCommand, AppUser>();

        // Delete User.
        CreateMap<DeleteUserRequest, DeleteUserCommand>();

        // Get User By ID.
        CreateMap<GetUserByIdRequest, GetUserByIdQuery>();

        // Get User Pagination.
        CreateMap<GetAllUsersPaginationRequest, GetAllUsersPaginationQuery>();
    }
}
