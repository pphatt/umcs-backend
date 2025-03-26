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
    IContributionPublicCommentRepository ContributionPublicCommentRepository { get; }
    IContributionCommentRepository ContributionCommentRepository { get; }
    IContributionPublicReadLaterRepository ContributionPublicReadLaterRepository { get; }
    IContributionPublicBookmarkRepository ContributionPublicBookmarkRepository { get; }
    IContributionPublicRatingRepository ContributionPublicRatingRepository { get; }
    IFileRepository FileRepository { get; }
    ILikeRepository LikeRepository { get; }
    ITagRepository TagRepository { get; }
    INotificationRepository NotificationRepository { get; }
    INotificationUserRepository NotificationUserRepository { get; }
    IPrivateChatRoomRepository PrivateChatRoomRepository { get; }
    IPrivateChatMessageRepository PrivateChatMessageRepository { get; }
}
