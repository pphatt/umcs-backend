using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.ContributionApp.Commands.RejectContribution;

public class RejectContributionCommandHandler : IRequestHandler<RejectContributionCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RejectContributionCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IConfiguration configuration,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(RejectContributionCommand request, CancellationToken cancellationToken)
    {
        var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(request.Id);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        if (!contribution.IsConfirmed)
        {
            return Errors.Contribution.NotConfirmed;
        }

        if (contribution.Status == ContributionStatus.Approve)
        {
            return Errors.Contribution.AlreadyApproved;
        }

        if (contribution.Status == ContributionStatus.Reject)
        {
            return Errors.Contribution.AlreadyRejected;
        }

        var coordinator = await _userManager.FindByIdAsync(request.CoordinatorId.ToString());

        if (coordinator is null)
        {
            return Errors.User.CoordinatorCannotFound;
        }

        var student = await _userManager.FindByIdAsync(contribution.UserId.ToString());

        if (student is null)
        {
            return Errors.User.CannotFound;
        }

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(student.FacultyId!.Value);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        if (coordinator.FacultyId != contribution.FacultyId)
        {
            return Errors.Contribution.NotBelongToFaculty;
        }

        await _unitOfWork.ContributionRepository.RejectContribution(contribution, request.CoordinatorId, request.Reason);

        var baseUrl = _configuration["ApplicationSettings:FrontendUrl"];
        var blogUrl = $"{baseUrl}/contribution/${contribution.Id}";

        var emailRequest = new MailRequest
        {
            ToEmail = student.Email!,
            Subject = "Reject Contribution",
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

        await _emailService.SendEmailAsync(emailRequest);

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Reject contribution successfully."
        };
    }
}
