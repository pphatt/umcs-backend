using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Identity;

namespace Server.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class RejectContributionJob : IJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RejectContributionJob> _logger;

    public RejectContributionJob(
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider,
        IConfiguration configuration,
        ILogger<RejectContributionJob> logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"----- Initialize checking current academic year expired and rejecting contribution in that academic year ----- {_dateTimeProvider.UtcNow}");

        var date = _dateTimeProvider.UtcNow;
        var currentAcademicYear = await _unitOfWork.AcademicYearRepository.GetAcademicYearByYearAsync(date);
        const string rejectReason = "Academic year is already closed, your contribution is rejected";

        if (currentAcademicYear is null || !currentAcademicYear.IsActive)
        {
            _logger.LogInformation($"----- Current academic year is already inactive ----- {_dateTimeProvider.UtcNow}");
            return;
        }

        if (currentAcademicYear.IsActive && currentAcademicYear.FinalClosureDate < date)
        {
            currentAcademicYear.IsActive = false;
        }

        await _unitOfWork.CompleteAsync();

        var pendingContributionsInCurrentYear = _unitOfWork.ContributionRepository
            .FindByCondition(x => x.AcademicYearId == currentAcademicYear.Id && x.Status == ContributionStatus.Pending)
            .ToList();

        if (!pendingContributionsInCurrentYear.Any())
        {
            _logger.LogInformation("No pending contributions found for academic year {YearId}", currentAcademicYear.Name);
            return;
        }

        var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);

        foreach (var contribution in pendingContributionsInCurrentYear)
        {
            foreach (var admin in admins)
            {
                await _unitOfWork.ContributionRepository.RejectContribution(contribution, admin.Id, rejectReason);

                var student = await _userManager.FindByIdAsync(contribution.UserId.ToString());
                var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(student!.FacultyId!.Value);

                var baseUrl = _configuration["ApplicationSettings:FrontendUrl"];
                var blogUrl = $"{baseUrl}/contribution/${contribution.Id}";

                var mail = new MailRequest
                {
                    ToEmail = student.Email,
                    Subject = "REJECT CONTRIBUTION",
                    Body = $@"
                                <!DOCTYPE html>
                                <html>
                                <head>
                                <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                                <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
                                <title>Contribution Rejected</title>
                                <style>
                                    @media only screen and (max-width: 600px) {{
                                        .container {{
                                            width: 100% !important;
                                        }}
                                    }}
                                </style>
                                </head>
                                <body style='background-color: #ffffff; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Oxygen-Sans, Ubuntu, Cantarell, ""Helvetica Neue"", sans-serif; margin: 0;'>
                                <div style='margin: 10px auto; width: 600px; max-width: 100%; border: 1px solid #E5E5E5;'>
                                    <!-- Tracking Section -->
                                    <div style='padding: 22px 40px; background-color: #F7F7F7;'>
                                        <table width='100%' cellpadding='0' cellspacing='0' role='presentation'>
                                            <tr>
                                                <td>
                                                    <p style='margin: 0; line-height: 2; font-weight: bold;'>Contribution ID number</p>
                                                    <p style='margin: 12px 0 0 0; font-weight: 500; line-height: 1.4; color: #6F6F6F;'>{contribution.Id}</p>
                                                </td>
                                                <td align='right'>
                                                    <a href='{blogUrl}' style='border: 1px solid #929292; font-size: 16px; text-decoration: none; padding: 10px 0px; width: 220px; display: block; text-align: center; font-weight: 500; color: #000;'>View Contribution</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
        
                                    <!-- Divider -->
                                    <hr style='border-color: #E5E5E5; margin: 0;' />
        
                                    <!-- Message Section -->
                                    <div style='padding: 40px 74px; text-align: center;'>
                                        <h1 style='font-size: 32px; line-height: 1.3; font-weight: 700; text-align: center; letter-spacing: -1px;'>Your Contribution has been Rejected!</h1>
                                        <p style='margin: 0; line-height: 2; color: #747474; font-weight: 500;'>Thank you for your submission.</p>
                                    </div>
        
                                    <!-- Divider -->
                                    <hr style='border-color: #E5E5E5; margin: 0;' />
        
                                    <!-- Details Section -->
                                    <div style='padding-left: 40px; padding-right: 40px; padding-top: 22px; padding-bottom: 22px;'>
                                        <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                            Title: <span style='font-weight: 500;'>{contribution.Title}</span>
                                        </div>
                                        <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                            User: <span style='font-weight: 500;'>{student.UserName}</span>
                                        </div>
                                        <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                            Faculty: <span style='font-weight: 500;'>{faculty.Name}</span>
                                        </div>
                                        <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                            Academic Year: <span style='font-weight: 500;'>{_dateTimeProvider.UtcNow.Year}-{_dateTimeProvider.UtcNow.Year + 1}</span>
                                        </div>
                                    </div>
                                </div>
                                </body>
                                </html>"
                };

                await _emailService.SendEmailAsync(mail);
            }
        }

        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"----- Finish checking current academic year expired and rejecting contribution in that academic year ----- {_dateTimeProvider.UtcNow}");
    }
}
