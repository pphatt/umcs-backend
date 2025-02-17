using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IFacultyRepository : IRepository<Faculty, Guid>
{
    Task<int> Count();

    Task<PaginationResult<FacultyDto>> GetAllFacultiesPagination(string? keyword, int pageIndex = 1, int pageSize = 10);

    Task<Faculty> GetFacultyByNameAsync(string Name);
}
