using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IAcademicYearRepository : IRepository<AcademicYear, Guid>
{
    Task<AcademicYear> GetAcademicYearByNameAsync(string academicYearName);
}
