using AutoMapper;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;
using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Queries.GetAcademicYearById;
using Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetAllContributionActivityLogsPagination;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogById;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogsByContributionId;
using Server.Application.Features.ContributionApp.Commands.ApproveContribution;
using Server.Application.Features.ContributionApp.Commands.CreateContribution;
using Server.Application.Features.ContributionApp.Commands.RejectContribution;
using Server.Application.Features.ContributionApp.Commands.UpdateContribution;
using Server.Application.Features.ContributionApp.Queries.GetAllContributionsPagination;
using Server.Application.Features.ContributionApp.Queries.GetAllUngradedContributionsPagination;
using Server.Application.Features.ContributionApp.Queries.GetContributionBySlug;
using Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;
using Server.Application.Features.ContributionCommentApp.Commands.CreateComment;
using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;
using Server.Application.Features.FacultyApp.Commands.CreateFaculty;
using Server.Application.Features.FacultyApp.Commands.DeleteFaculty;
using Server.Application.Features.FacultyApp.Commands.UpdateFaculty;
using Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;
using Server.Application.Features.FacultyApp.Queries.GetFacultyById;
using Server.Application.Features.Identity.Commands.BulkDeleteUsers;
using Server.Application.Features.Identity.Commands.ChangeUserAvatar;
using Server.Application.Features.Identity.Commands.CreateGuest;
using Server.Application.Features.Identity.Commands.DeleteUser;
using Server.Application.Features.Identity.Commands.EditUserProfile;
using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Application.Features.Identity.Commands.ResetPassword;
using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Application.Features.Identity.Commands.UploadUserAvatar;
using Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;
using Server.Application.Features.Identity.Queries.GetAllUsersPagination;
using Server.Application.Features.Identity.Queries.GetUserById;
using Server.Application.Features.Identity.Queries.GetUserProfile;
using Server.Application.Features.Notification.Commands.BulkDeleteNotifications;
using Server.Application.Features.Notification.Commands.DeleteNotification;
using Server.Application.Features.Notification.Commands.MarkNotificationAsRed;
using Server.Application.Features.Notification.Commands.UnreadNotification;
using Server.Application.Features.Notification.Queries.GetAllUserNotificationsPagination;
using Server.Application.Features.Notification.Queries.GetNotificationById;
using Server.Application.Features.PrivateChatApp.Commands.MarkMessageAsRed;
using Server.Application.Features.PrivateChatApp.Commands.SendChatMessage;
using Server.Application.Features.PrivateChatApp.Queries.GetAllRoomsPagination;
using Server.Application.Features.PrivateChatApp.Queries.GetUserChatMessagesPagination;
using Server.Application.Features.PublicContributionApp.Commands.AllowGuest;
using Server.Application.Features.PublicContributionApp.Commands.AllowGuestWithManyContributions;
using Server.Application.Features.PublicContributionApp.Commands.RatePublicContribution;
using Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuest;
using Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuestWithManyContributions;
using Server.Application.Features.PublicContributionApp.Commands.ToggleBookmarkContribution;
using Server.Application.Features.PublicContributionApp.Commands.ToggleLikeContribution;
using Server.Application.Features.PublicContributionApp.Commands.ToggleReadLater;
using Server.Application.Features.PublicContributionApp.Queries.DownloadAllFiles;
using Server.Application.Features.PublicContributionApp.Queries.DownloadSingleFile;
using Server.Application.Features.PublicContributionApp.Queries.GetAllBookmarkPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetAllPublicContributionsPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetAllReadLaterPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetAllUserLikePublicContributionsPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetLatestPublicContributions;
using Server.Application.Features.PublicContributionApp.Queries.GetListUserLiked;
using Server.Application.Features.PublicContributionApp.Queries.GetPublicContributionBySlug;
using Server.Application.Features.PublicContributionApp.Queries.GetTopContributors;
using Server.Application.Features.PublicContributionApp.Queries.GetTopMostLikedPublicContributions;
using Server.Application.Features.PublicContributionApp.Queries.GetTopMostViewedPublicContributions;
using Server.Application.Features.PublicContributionApp.Queries.GetTopRatedPublicContributions;
using Server.Application.Features.PublicContributionCommentApp.Commands;
using Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear;
using Server.Application.Features.Role.Commands.BulkDeleteRoles;
using Server.Application.Features.Role.Commands.CreateRole;
using Server.Application.Features.Role.Commands.DeleteRole;
using Server.Application.Features.Role.Commands.SavePermissionsToRole;
using Server.Application.Features.Role.Commands.UpdateRole;
using Server.Application.Features.Role.Queries.GetAllRolePermissions;
using Server.Application.Features.Role.Queries.GetAllRolesPagination;
using Server.Application.Features.Role.Queries.GetRoleById;
using Server.Application.Features.TagApp.Commands.BulkDeleteTags;
using Server.Application.Features.TagApp.Commands.CreateTag;
using Server.Application.Features.TagApp.Commands.DeleteTag;
using Server.Application.Features.TagApp.Commands.UpdateTag;
using Server.Application.Features.TagApp.Queries.GetAllTagsPagination;
using Server.Application.Features.TagApp.Queries.GetTagById;
using Server.Application.Features.Users.Commands.CreateUser;
using Server.Contracts.AcademicYears.ActivateAcademicYear;
using Server.Contracts.AcademicYears.BulkDeleteAcademicYears;
using Server.Contracts.AcademicYears.CreateAcademicYear;
using Server.Contracts.AcademicYears.DeleteAcademicYear;
using Server.Contracts.AcademicYears.GetAcademicYearById;
using Server.Contracts.AcademicYears.GetAllAcademicYearsPagination;
using Server.Contracts.AcademicYears.InactivateAcademicYear;
using Server.Contracts.AcademicYears.UpdateAcademicYear;
using Server.Contracts.Authentication.RefreshToken;
using Server.Contracts.Common.Media;
using Server.Contracts.ContributionActivityLogs.GetAllContributionActivityLogsPagination;
using Server.Contracts.ContributionActivityLogs.GetContributionActivityLogById;
using Server.Contracts.ContributionActivityLogs.GetContributionActivityLogsByContributionId;
using Server.Contracts.ContributionComments.CreateComment;
using Server.Contracts.Contributions.ApproveContribution;
using Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;
using Server.Contracts.Contributions.CreateContribution;
using Server.Contracts.Contributions.GetAllUngradedContributionsPagination;
using Server.Contracts.Contributions.GetContributionBySlug;
using Server.Contracts.Contributions.GetPersonalContributionDetailBySlug;
using Server.Contracts.Contributions.RejectContribution;
using Server.Contracts.Contributions.UpdateContribution;
using Server.Contracts.Faculties.BulkDeleteFaculties;
using Server.Contracts.Faculties.CreateFaculty;
using Server.Contracts.Faculties.DeleteFaculty;
using Server.Contracts.Faculties.GetAllFacultiesPagination;
using Server.Contracts.Faculties.GetFacultyById;
using Server.Contracts.Faculties.UpdateFaculty;
using Server.Contracts.Identity.BulkDeleteUsers;
using Server.Contracts.Identity.ChangeUserAvatar;
using Server.Contracts.Identity.CreateGuest;
using Server.Contracts.Identity.CreateUser;
using Server.Contracts.Identity.DeleteUser;
using Server.Contracts.Identity.EditUserProfile;
using Server.Contracts.Identity.ForgotPassword;
using Server.Contracts.Identity.GetAllUsersPagination;
using Server.Contracts.Identity.GetUserById;
using Server.Contracts.Identity.GetUserProfile;
using Server.Contracts.Identity.ResetPassword;
using Server.Contracts.Identity.UpdateUser;
using Server.Contracts.Identity.UploadUserAvatar;
using Server.Contracts.Identity.ValidateForgotPasswordToken;
using Server.Contracts.Notifications.BulkDeleteNotifications;
using Server.Contracts.Notifications.DeleteNotification;
using Server.Contracts.Notifications.GetAllUserNotificationsPagination;
using Server.Contracts.Notifications.GetNotificationById;
using Server.Contracts.Notifications.MarkNotificationAsRed;
using Server.Contracts.Notifications.UnreadNotification;
using Server.Contracts.PrivateChats.GetAllChatRoomsPagination;
using Server.Contracts.PrivateChats.GetUserChatMessagesPagination;
using Server.Contracts.PrivateChats.MarkMessageAsRed;
using Server.Contracts.PrivateChats.SendChatMessage;
using Server.Contracts.PublicContributionComments.CreatePublicComment;
using Server.Contracts.PublicContributions.AllowGuest;
using Server.Contracts.PublicContributions.AllowGuestWithManyContributions;
using Server.Contracts.PublicContributions.DownloadAllFiles;
using Server.Contracts.PublicContributions.DownloadSingleFile;
using Server.Contracts.PublicContributions.GetAllBookmarkPagination;
using Server.Contracts.PublicContributions.GetAllPublicContributionsPagination;
using Server.Contracts.PublicContributions.GetAllReadLaterPagination;
using Server.Contracts.PublicContributions.GetAllUserLikePublicContributionsPagination;
using Server.Contracts.PublicContributions.GetAllUsersLikedContributionPagination;
using Server.Contracts.PublicContributions.GetLatestPublicContributions;
using Server.Contracts.PublicContributions.GetPublicContributionBySlug;
using Server.Contracts.PublicContributions.GetTopContributors;
using Server.Contracts.PublicContributions.GetTopMostLikedPublicContributions;
using Server.Contracts.PublicContributions.GetTopMostViewedPublicContributions;
using Server.Contracts.PublicContributions.GetTopRatedPublicContributions;
using Server.Contracts.PublicContributions.RatePublicContribution;
using Server.Contracts.PublicContributions.RevokeAllowGuest;
using Server.Contracts.PublicContributions.RevokeAllowGuestWithManyContributions;
using Server.Contracts.PublicContributions.ToggleBookmarkContribution;
using Server.Contracts.PublicContributions.ToggleLikeContribution;
using Server.Contracts.PublicContributions.ToggleReadLater;
using Server.Contracts.Report.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear;
using Server.Contracts.Roles.BulkDeleteRoles;
using Server.Contracts.Roles.CreateRole;
using Server.Contracts.Roles.DeleteRole;
using Server.Contracts.Roles.GetAllRolePermissions;
using Server.Contracts.Roles.GetAllRolesPagination;
using Server.Contracts.Roles.GetRoleById;
using Server.Contracts.Roles.UpdateRole;
using Server.Contracts.Tags.BulkDeleteTags;
using Server.Contracts.Tags.CreateTag;
using Server.Contracts.Tags.DeleteTag;
using Server.Contracts.Tags.GetAllTagsPagination;
using Server.Contracts.Tags.GetTagById;
using Server.Contracts.Tags.UpdateTag;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

using File = Server.Domain.Entity.Content.File;
using LoginRequest = Server.Contracts.Authentication.Login.LoginRequest;

namespace Server.Api.Common.Mapper;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        // Authentication.
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();

        // User.
        CreateMap<AppUser, UserDto>().ReverseMap();

        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<CreateUserCommand, AppUser>();
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UpdateUserCommand, AppUser>();
        CreateMap<DeleteUserRequest, DeleteUserCommand>();
        CreateMap<BulkDeleteUsersRequest, BulkDeleteUsersCommand>();

        CreateMap<GetUserByIdRequest, GetUserByIdQuery>();
        CreateMap<GetAllUsersPaginationRequest, GetAllUsersPaginationQuery>();

        CreateMap<ForgotPasswordRequest, ForgotPasswordCommand>();
        CreateMap<ResetPasswordRequest, ResetPasswordCommand>();
        CreateMap<ValidateForgotPasswordTokenRequest, ValidateForgotPasswordTokenCommand>();

        CreateMap<CreateGuestRequest, CreateGuestCommand>();
        CreateMap<CreateGuestCommand, AppUser>();

        CreateMap<GetUserProfileRequest, GetUserProfileQuery>();
        CreateMap<AppUser, UserProfileDto>();

        CreateMap<UploadUserAvatarRequest, UploadUserAvatarCommand>();

        CreateMap<EditUserProfileRequest, EditUserProfileCommand>();

        CreateMap<ChangeUserAvatarRequest, ChangeUserAvatarCommand>();

        // Role.
        CreateMap<AppRole, RoleDto>().ReverseMap();

        CreateMap<CreateRoleRequest, CreateRoleCommand>();
        CreateMap<UpdateRoleRequest, UpdateRoleCommand>();
        CreateMap<DeleteRoleRequest, DeleteRoleCommand>();
        CreateMap<BulkDeleteRolesRequest, BulkDeleteRolesCommand>();

        CreateMap<GetRoleByIdRequest, GetRoleByIdQuery>();
        CreateMap<GetAllRolesPaginationRequest, GetAllRolesPaginationQuery>();

        CreateMap<GetAllRolePermissionsRequest, GetAllRolePermissionsQuery>();
        CreateMap<PermissionsDto, SavePermissionsToRoleCommand>();

        // Faculty.
        CreateMap<Faculty, FacultyDto>().ReverseMap();

        CreateMap<CreateFacultyRequest, CreateFacultyCommand>();
        CreateMap<UpdateFacultyRequest, UpdateFacultyCommand>();
        CreateMap<DeleteFacultyRequest, DeleteFacultyCommand>();
        CreateMap<BulkDeleteFacultiesRequest, BulkDeleteFacultiesCommand>();

        CreateMap<GetFacultyByIdRequest, GetFacultyByIdQuery>();
        CreateMap<GetAllFacultiesPaginationRequest, GetAllFacultiesPaginationQuery>();

        CreateMap<Faculty, FacultyDto>();

        // Academic Year.
        CreateMap<AcademicYear, AcademicYearDto>().ReverseMap();

        CreateMap<CreateAcademicYearRequest, CreateAcademicYearCommand>();
        CreateMap<UpdateAcademicYearRequest, UpdateAcademicYearCommand>();
        CreateMap<DeleteAcademicYearRequest, DeleteAcademicYearCommand>();
        CreateMap<BulkDeleteAcademicYearsRequest, BulkDeleteAcademicYearsCommand>();

        CreateMap<GetAcademicYearByIdRequest, GetAcademicYearByIdQuery>();
        CreateMap<GetAllAcademicYearsPaginationRequest, GetAllAcademicYearsPaginationQuery>();

        CreateMap<ActivateAcademicYearRequest, ActivateAcademicYearCommand>();
        CreateMap<InactivateAcademicYearRequest, InactivateAcademicYearCommand>();

        // Contribution.
        CreateMap<CreateContributionRequest, CreateContributionCommand>();
        CreateMap<UpdateContributionRequest, UpdateContributionCommand>();
        CreateMap<UpdateContributionCommand, Contribution>()
            .ForMember(dest => dest.Files, opt => opt.Ignore())
            .ForMember(dest => dest.AcademicYearId, opt => opt.Ignore())
            .ForMember(dest => dest.AcademicYear, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.SubmissionDate, opt => opt.Ignore())
            .ForMember(dest => dest.PublicDate, opt => opt.Ignore())
            .ForMember(dest => dest.Faculty, opt => opt.Ignore())
            .ForMember(dest => dest.ContributionTags, opt => opt.Ignore());

        CreateMap<GetAllContributionsPaginationRequest, GetAllContributionsPaginationQuery>();

        CreateMap<ApproveContributionRequest, ApproveContributionCommand>();
        CreateMap<RejectContributionRequest, RejectContributionCommand>();

        CreateMap<GetContributionBySlugRequest, GetContributionBySlugQuery>();

        CreateMap<GetAllUngradedContributionsPaginationRequest, GetAllUngradedContributionsPaginationQuery>();

        CreateMap<GetPersonalContributionDetailBySlugRequest, GetPersonalContributionDetailBySlugQuery>();

        CreateMap<ContributionDto, ContributionWithCommentDto>();

        // Contribution Comment.
        CreateMap<CreateCommentRequest, CreateCommentCommand>();

        // Public Contribution.
        CreateMap<AllowGuestRequest, AllowGuestCommand>();
        CreateMap<Contribution, ContributionPublic>();

        CreateMap<RevokeAllowGuestRequest, RevokeAllowGuestCommand>();

        CreateMap<AllowGuestWithManyContributionsRequest, AllowGuestWithManyContributionsCommand>();
        CreateMap<RevokeAllowGuestWithManyContributionsRequest, RevokeAllowGuestWithManyContributionsCommand>();

        CreateMap<GetAllPublicContributionsPaginationRequest, GetAllPublicContributionsPaginationQuery>();

        CreateMap<GetPublicContributionBySlugRequest, GetPublicContributionBySlugQuery>();

        CreateMap<DownloadSingleFileRequest, DownloadSingleFileQuery>();
        CreateMap<DownloadAllFilesRequests, DownloadAllFilesQuery>();

        CreateMap<PublicContributionDto, PublicContributionWithCommentsDto>();

        CreateMap<GetLatestPublicContributionsRequest, GetLatestPublicContributionsQuery>();

        CreateMap<GetTopMostLikedPublicContributionsRequest, GetTopMostLikedPublicContributionsQuery>();

        CreateMap<GetTopMostViewedPublicContributionsRequest, GetTopMostViewedPublicContributionsQuery>();

        CreateMap<GetTopContributorsRequest, GetTopContributorsQuery>();

        // Public Contribution Comment.
        CreateMap<CreatePublicCommentRequest, CreatePublicCommentCommand>();

        // Like.
        CreateMap<ToggleLikeContributionRequest, ToggleLikeContributionCommand>();

        CreateMap<GetAllUsersLikedContributionPaginationRequest, GetAllUsersLikedContributionPaginationQuery>();

        CreateMap<GetAllUserLikePublicContributionsPaginationRequest, GetAllUserLikePublicContributionsPaginationQuery>();

        // Read Later.
        CreateMap<ToggleReadLaterRequest, ToggleReadLaterCommand>();

        CreateMap<GetAllReadLaterPaginationRequest, GetAllReadLaterPaginationQuery>();

        // Bookmark.
        CreateMap<ToggleBookmarkContributionRequest, ToggleBookmarkContributionCommand>();

        CreateMap<GetAllBookmarkPaginationRequest, GetAllBookmarkPaginationQuery>();

        // Rate.
        CreateMap<RatePublicContributionRequest, RatePublicContributionCommand>();

        CreateMap<GetTopRatedPublicContributionsRequest, GetTopRatedPublicContributionsQuery>();

        // Tag.
        CreateMap<Tag, TagDto>();
        CreateMap<Tag, TagInListDto>();

        CreateMap<CreateTagRequest, CreateTagCommand>();

        CreateMap<UpdateTagRequest, UpdateTagCommand>();

        CreateMap<DeleteTagRequest, DeleteTagCommand>();

        CreateMap<BulkDeleteTagsRequest, BulkDeleteTagsCommand>();

        CreateMap<GetTagByIdRequest, GetTagByIdQuery>();

        CreateMap<GetAllTagsPaginationRequest, GetAllTagsPaginationQuery>();

        // Contribution Activity.
        CreateMap<ContributionActivityLog, ContributionActivityLogDto>();

        CreateMap<GetAllContributionActivityLogsPaginationRequest, GetAllContributionActivityLogsPaginationQuery>();
        CreateMap<GetContributionActivityLogByIdRequest, GetContributionActivityLogByIdQuery>();

        CreateMap<GetContributionActivityLogsByContributionIdRequest, GetContributionActivityLogsByContributionIdQuery>();

        // File.
        CreateMap<File, DeleteFilesRequest>();

        // Notification
        CreateMap<Notification, NotificationDto>();

        CreateMap<GetAllUserNotificationsPaginationRequest, GetAllUserNotificationsPaginationQuery>();
        CreateMap<MarkNotificationAsRedRequest, MarkNotificationAsRedCommand>();
        CreateMap<UnreadNotificationRequest, UnreadNotificationCommand>();
        CreateMap<DeleteNotificationRequest, DeleteNotificationCommand>();
        CreateMap<BulkDeleteNotificationsRequest, BulkDeleteNotificationsCommand>();
        CreateMap<GetNotificationByIdRequest, GetNotificationByIdQuery>();

        // Private Chat
        CreateMap<GetAllChatRoomsPaginationRequest, GetAllChatRoomsPaginationQuery>();
        CreateMap<SendChatMessageRequest, SendChatMessageCommand>();
        CreateMap<GetUserChatMessagesPaginationRequest, GetUserChatMessagesPaginationQuery>();
        CreateMap<MarkMessageAsRedRequest, MarkMessageAsRedCommand>();

        // Report
        CreateMap<GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearRequest, GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQuery>();
    }
}
