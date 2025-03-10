using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface ITagRepository : IRepository<Tag, Guid>
{
    Task<PaginationResult<TagInListDto>> GetAllTagsPagination(string? keyword, int pageIndex = 1, int pageSize = 10);

    Task<Tag> GetTagByName(string name);
}
