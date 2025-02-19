using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;
using File = Server.Domain.Entity.Content.File;

namespace Server.Application.Features.ContributionApp.Commands.CreateContribution;

public class CreateContributionCommandHandler : IRequestHandler<CreateContributionCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMediaService _mediaService;
    private readonly IEmailService _emailService;

    public CreateContributionCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMediaService mediaService, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _userManager = userManager;
        _roleManager = roleManager;
        _mediaService = mediaService;
        _emailService = emailService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(CreateContributionCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        if (!request.IsConfirmed)
        {
            return Errors.Contribution.NotConfirmed;
        }

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        var academicYear = await _unitOfWork.AcademicYearRepository.GetAcademicYearByDateAsync(_dateTimeProvider.UtcNow);

        if (academicYear is null)
        {
            return Errors.Contribution.AcademicYearNotFound;
        }

        var isInTimeAcademicYear = await _unitOfWork.AcademicYearRepository.CanSubmitAsync(_dateTimeProvider.UtcNow);

        if (!isInTimeAcademicYear)
        {
            return Errors.Contribution.CannotSubmit;
        }

        var contribution = new Contribution
        {
            Id = Guid.NewGuid(),
            FacultyId = faculty.Id,
            AcademicYearId = academicYear.Id,
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            ShortDescription = request.ShortDescription,
            SubmissionDate = _dateTimeProvider.UtcNow,
            Status = ContributionStatus.Pending,
            IsConfirmed = true,
        };

        _unitOfWork.ContributionRepository.Add(contribution);

        // handle upload thumbnail.
        if (request.Thumbnail is not null)
        {
            var files = new List<IFormFile> { request.Thumbnail };
            var required = new FileRequiredParamsDto
            {
                type = FileType.Thumbnail,
                userId = user.Id,
                contributionId = contribution.Id,
            };

            var filesInfo = await _mediaService.UploadFilesToCloudinary(files, required);

            foreach (var info in filesInfo)
            {
                var file = new File
                {
                    Id = Guid.NewGuid(),
                    ContributionId = contribution.Id,
                    Type = FileType.Thumbnail,
                    Name = info.Name,
                    Path = info.Path,
                    PublicId = info.PublicId,
                    Extension = info.Extension,
                };

                _unitOfWork.FileRepository.Add(file);
            }
        }

        // handle upload files.
        if (request.Files is not null && request.Files.Count() > 0)
        {
            var required = new FileRequiredParamsDto
            {
                type = FileType.File,
                userId = user.Id,
                contributionId = contribution.Id
            };

            var filesInfo = await _mediaService.UploadFilesToCloudinary(request.Files, required);

            foreach (var info in filesInfo)
            {
                var file = new File
                {
                    Id= Guid.NewGuid(),
                    ContributionId = contribution.Id,
                    Type = FileType.File,
                    Name = info.Name,
                    Path = info.Path,
                    PublicId = info.PublicId,
                    Extension = info.Extension,
                };

                _unitOfWork.FileRepository.Add(file);
            }
        }

        var coordinators = await _userManager.FindUserInRoleByFacultyIdAsync(_roleManager, Roles.Coordinator, request.FacultyId);

        foreach (var coordinator in coordinators)
        {
            await _emailService.SendEmailAsync(new MailRequest
            {
                ToEmail = coordinator.Email!,
                Subject = "New Contribution Submit.",
                Body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
                    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
                    <title>Submitted contribution successfully</title>
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
                                </tr>
                            </table>
                        </div>
        
                        <!-- Divider -->
                        <hr style='border-color: #E5E5E5; margin: 0;' />
        
                        <!-- Message Section -->
                        <div style='padding: 40px 74px; text-align: center;'>
                            <h1 style='font-size: 32px; line-height: 1.3; font-weight: 700; text-align: center; letter-spacing: -1px;'>New Contribution request is pending.</h1>
                            <p style='margin: 0; line-height: 2; color: #747474; font-weight: 500;'>Wait around 14 days to get accepted.</p>
                        </div>
        
                        <!-- Divider -->
                        <hr style='border-color: #E5E5E5; margin: 0;' />
        
                        <!-- Details Section -->
                        <div style='padding-left: 40px; padding-right: 40px; padding-top: 22px; padding-bottom: 22px;'>
                            <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                Title: <span style='font-weight: 500;'>{contribution.Title}</span>
                            </div>
                            <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                Upload Date: <span style='font-weight: 500;'>{DateTime.Now.ToString("MMMM dd, yyyy")}</span>
                            </div>
                            <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                Faculty: <span style='font-weight: 500;'>{faculty.Name}</span>
                            </div>
                            <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                Academic Year: <span style='font-weight: 500;'>{academicYear.Name}</span>
                            </div>
                            <div style='margin: 0; line-height: 2; font-size: 15px; font-weight: bold;'>
                                Submitted By: <span style='font-weight: 500;'>{user.UserName}</span>
                            </div>
                        </div>
                    </div>
                    </body>
                    </html>"
            });
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Create contribution successfully."
        };
    }
}
