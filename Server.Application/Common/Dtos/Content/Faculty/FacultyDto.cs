using AutoMapper;

namespace Server.Application.Common.Dtos.Content.Faculty;

using Faculty = Domain.Entity.Content.Faculty;

public class FacultyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Faculty, FacultyDto>();
        }
    }
}
