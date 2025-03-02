using Server.Application.Common.Interfaces.Persistence.Repositories;

namespace Server.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task<int> CompleteAsync();

    ITokenRepository TokenRepository { get; }
    IFacultyRepository FacultyRepository { get; }
    IAcademicYearRepository AcademicYearRepository { get; }
    IContributionRepository ContributionRepository { get; }
    IContributionPublicRepository ContributionPublicRepository { get; }
    IContributionActivityLogRepository ContributionActivityLogRepository { get; }
    IFileRepository FileRepository { get; }
    ILikeRepository LikeRepository { get; }
}
