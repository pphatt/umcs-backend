using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Hubs.Notifications;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.ContributionCommentApp.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public CreateCommentCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IHubContext<NotificationHub> notificationHub)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _notificationHub = notificationHub;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        if (contribution.FacultyId != user.FacultyId)
        {
            return Errors.Contribution.NotBelongToFaculty;
        }

        if (contribution.Status != ContributionStatus.Pending)
        {
            return Errors.Contribution.CannotCommentOnContributionAlreadyGraded;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Guest))
        {
            return Errors.Contribution.NotPublicYet;
        }

        if (role.Contains(Roles.Student) && user.Id != contribution.UserId)
        {
            return Errors.Contribution.NotBelongTo;
        }

        _unitOfWork.ContributionCommentRepository.Add(new ContributionComment
        {
            Content = request.Content,
            ContributionId = request.ContributionId,
            UserId = request.UserId
        });

        if (role.Contains(Roles.Coordinator))
        {
            if (!contribution.IsCoordinatorCommented)
            {
                contribution.IsCoordinatorCommented = true;

                var owner = await _userManager.FindByIdAsync(contribution.UserId.ToString());

                await _emailService.SendEmailAsync(new MailRequest
                {
                    ToEmail = owner.Email,
                    Subject = "Coordinator comment.",
                    Body = "Coordinator have commented on your contribution"
                });
            }
        }

        await _unitOfWork.CompleteAsync();

        var notification = new Server.Domain.Entity.Content.Notification
        {
            Title = "Contribution have new comment",
            DateCreated = DateTime.Now,
            Content = $"Contribution has been commented",
            UserId = user.Id,
            Type = "Contribution-Private-Comment",
            Username = user.UserName ?? "username",
            Avatar = user.Avatar ?? "",
            Slug = contribution.Slug
        };

        var notificationDto = new NotificationDto
        {
            Title = "Contribution have new comment",
            DateCreated = DateTime.Now,
            Content = $"Contribution has been commented",
            Type = "Contribution-Private-Comment",
            Username = user.UserName ?? "username",
            Avatar = user.Avatar ?? "",
        };

        _unitOfWork.NotificationRepository.Add(notification);

        var notificationUser = new NotificationUser
        {
            NotificationId = notification.Id,
            UserId = contribution.UserId,
            HasRed = false
        };

        _unitOfWork.NotificationUserRepository.Add(notificationUser);

        await _unitOfWork.CompleteAsync();

        await _notificationHub
            .Clients
            .User(contribution.UserId.ToString())
            .SendAsync("GetNewNotification", notificationDto);

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = $"Comment successfully {contribution.UserId.ToString()}."
        };
    }
}
