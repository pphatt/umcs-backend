using AutoMapper;
using Server.Domain.Entity.Identity;

namespace Server.Application.Common.Dtos.Identity.Role;

public class RoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppRole, RoleDto>();
        }
    }
}
