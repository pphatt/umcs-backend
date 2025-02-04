using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using Server.Application.Features.Authentication.Commands.Login;

namespace Server.Api.Common.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile() 
    {
        // Authentication.
        CreateMap<LoginRequest, LoginCommand>();
    }
}
