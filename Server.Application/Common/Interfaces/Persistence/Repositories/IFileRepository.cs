namespace Server.Application.Common.Interfaces.Persistence.Repositories;

using File = Domain.Entity.Content.File;

public interface IFileRepository : IRepository<File, Guid>
{
    Task<List<File>> GetByContributionIdAsync(Guid contributionId);

    Task<List<File>> GetByListContributionIdsAsync(List<Guid> contributionIds);

    Task<List<string>> GetFilesPathByContributionId(Guid contributionId);
}
