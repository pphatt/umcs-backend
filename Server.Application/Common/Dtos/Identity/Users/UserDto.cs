using AutoMapper;
using Server.Domain.Entity.Identity;

namespace Server.Application.Common.Dtos.Identity.Users;

public class UserDto
{
    public Guid Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Faculty { get; set; }

    public bool IsActive { get; set; }

    public DateTime? DateCreated { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();

    public DateTime? Dob { get; set; }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, UserDto>();
        }
    }
}
