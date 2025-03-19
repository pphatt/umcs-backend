using Microsoft.Extensions.Caching.Memory;

using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Infrastructure.Services;

namespace Server.Infrastructure.Persistence.Repositories;

using Faculty = Server.Domain.Entity.Content.Faculty;

public class CacheFacultyRepository : CacheService<Faculty, Guid>, IFacultyRepository
{
    private readonly FacultyRepository _facultyRepository;
    private const string CacheKeyPrefix = "faculty:";

    public CacheFacultyRepository(FacultyRepository decorator, IMemoryCache memoryCache)
        : base(decorator, memoryCache, CacheKeyPrefix)
    {
        _facultyRepository = decorator;
    }

    public override Task<IEnumerable<Faculty>> GetAllAsync()
    {
        return GetOrCreateAsync("all", () => _facultyRepository.GetAllAsync());
    }

    public override Task<Faculty> GetByIdAsync(Guid id)
    {
        return GetOrCreateAsync($"id-{id}", () => _facultyRepository.GetByIdAsync(id));
    }

    public Task<int> Count()
    {
        return GetOrCreateAsync("count", () => _facultyRepository.Count());
    }

    public Task<PaginationResult<FacultyDto>> GetAllFacultiesPagination(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        return GetOrCreateAsync($"pagination-{keyword ?? "all"}-{pageIndex}-{pageSize}",
            () => _facultyRepository.GetAllFacultiesPagination(keyword, pageIndex, pageSize));
    }

    public Task<Faculty> GetFacultyByNameAsync(string name)
    {
        return GetOrCreateAsync($"by-name-{name}", () => _facultyRepository.GetFacultyByNameAsync(name));
    }
}
