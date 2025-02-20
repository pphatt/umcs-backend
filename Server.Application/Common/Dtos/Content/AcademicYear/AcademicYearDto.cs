using AutoMapper;

namespace Server.Application.Common.Dtos.Content.AcademicYear;

using AcademicYear = Domain.Entity.Content.AcademicYear;

public class AcademicYearDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime StartClosureDate { get; set; }

    public DateTime EndClosureDate { get; set; }

    public DateTime FinalClosureDate { get; set; }

    public bool IsActive { get; set; } = false;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AcademicYear, AcademicYearDto>();
        }
    }
}
