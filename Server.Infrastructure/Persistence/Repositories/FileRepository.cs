using Server.Application.Common.Interfaces.Persistence.Repositories;
using File = Server.Domain.Entity.Content.File;

namespace Server.Infrastructure.Persistence.Repositories;

public class FileRepository : RepositoryBase<File, Guid>, IFileRepository
{
    public FileRepository(AppDbContext context) : base(context)
    {
    }
}
