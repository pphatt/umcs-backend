namespace Server.Application.Common.Interfaces.Persistence.Repositories;

using File = Domain.Entity.Content.File;

public interface IFileRepository : IRepository<File, Guid>
{
}
