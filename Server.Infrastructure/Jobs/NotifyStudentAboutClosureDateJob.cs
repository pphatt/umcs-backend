using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Quartz;

using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Features.Notification.Hubs;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class NotifyStudentAboutClosureDateJob : IJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<NotifyStudentAboutClosureDateJob> _logger;

    private readonly int[] _reminderDays = new int[] { 7, 3, 1 };

    public NotifyStudentAboutClosureDateJob(
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IHubContext<NotificationHub> notificationHub,
        IDateTimeProvider dateTimeProvider,
        ILogger<NotifyStudentAboutClosureDateJob> logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _notificationHub = notificationHub;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"----- Initialize notify student about academic year expiration date ----- {_dateTimeProvider.UtcNow}");

        var currentDate = _dateTimeProvider.UtcNow;
        var currentAcademicYear = await _unitOfWork.AcademicYearRepository.GetAcademicYearByYearAsync(currentDate);

        if (currentAcademicYear is null || !currentAcademicYear.IsActive)
        {
            _logger.LogInformation("No active academic year found or academic year is not active");
            return;
        }

        var closureDate = currentAcademicYear.EndClosureDate;

        var daysRemaining = (closureDate - currentDate).Days;

        if (!Array.Exists(_reminderDays, day => day == daysRemaining))
        {
            _logger.LogInformation($"Today is not a reminder day. Days remaining: {daysRemaining}");
            return;
        }

        _logger.LogInformation($"Sending reminder emails for {daysRemaining} days remaining until closure date");

        var subject = $"REMINDER: {daysRemaining} {(daysRemaining > 1 ? "DAYS" : "")} LEFT FOR SUBMISSIONS";
        var message = CreateReminderMessage(daysRemaining, closureDate);

        var students = await _userManager.GetUsersInRoleAsync(Roles.Student);

        foreach (var student in students)
        {
            var mail = new MailRequest
            {
                ToEmail = student.Email,
                Subject = subject,
                Body = message
            };

            await _emailService.SendEmailAsync(mail);

            _logger.LogInformation($"Sent reminder email to {student.Email}");

            var notification = new Notification
            {
                UserId = Guid.NewGuid(),
                Username = "System",
                Avatar = "",
                Title = "Academic year day remain",
                DateCreated = DateTime.Now,
                Content = subject,
                Type = "AcademicYear-ExpiredDate",
                Slug = currentAcademicYear.Name,
            };

            var notificationDto = new NotificationDto
            {
                Username = "System",
                Avatar = "",
                Title = "Contribution rejected",
                DateCreated = DateTime.Now,
                Content = "Contribution has been rejected",
                Type = "Contribution-RejectContribution",
                HasRed = false
            };

            var notificationUser = new NotificationUser
            {
                UserId = student.Id,
                NotificationId = notification.Id,
                HasRed = false,
            };

            _unitOfWork.NotificationRepository.Add(notification);

            _unitOfWork.NotificationUserRepository.Add(notificationUser);

            await _unitOfWork.CompleteAsync();

            await _notificationHub
                .Clients
                .User(student.Id.ToString())
                .SendAsync("GetNewNotification", notificationDto);
        }

        _logger.LogInformation($"----- Finish notify student about academic year expiration date ----- {_dateTimeProvider.UtcNow}");
    }

    private string CreateReminderMessage(int daysRemaining, DateTime closureDate)
    {
        string urgency;

        if (daysRemaining == 7)
            urgency = "Please note";
        else if (daysRemaining == 3)
            urgency = "Important reminder";
        else if (daysRemaining == 1)
            urgency = "Final reminder";
        else
            urgency = "Reminder";

        return $@"<html>
                <body>
                    <h2>University Magazine Submission Reminder</h2>
                    <p>{urgency}: The deadline for new contributions to the university magazine is in <strong>{daysRemaining} {(daysRemaining > 1 ? "days" : "")}</strong>.</p>
                    <p>The submission system will close for new entries on <strong>{closureDate:dddd, MMMM d, yyyy}</strong> at <strong>{closureDate:h:mm tt}</strong>.</p>
                    <p>If you wish to contribute to this year's magazine, please submit your articles or photographs before this deadline.</p>
                    <p>Note that after the closure date, you will still be able to update existing submissions until the final closure date.</p>
                    <p>Thank you for your participation!</p>
                    <p>University Magazine Team</p>
                </body>
                </html>";
    }
}
