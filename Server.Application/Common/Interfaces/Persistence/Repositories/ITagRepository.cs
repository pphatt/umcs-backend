using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface ITagRepository : IRepository<Tag, Guid>
{
    Task<Tag> GetTagByName(string name);
}
