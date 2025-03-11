using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Identity;

namespace Server.Infrastructure.Jobs;

public class MailCoordinatorJob : IJob
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<MailCoordinatorJob> _logger;

    public MailCoordinatorJob(
        AppDbContext context,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IConfiguration configuration,
        IDateTimeProvider dateTimeProvider,
        ILogger<MailCoordinatorJob> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"----- Initialize send email to all coordinator ----- {_dateTimeProvider.UtcNow}");

        var query = from c in _context.Contributions
                    where c.DateDeleted == null && c.Status == ContributionStatus.Pending && c.IsCoordinatorCommented == false
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    group new { c, f } by f.Id into facultyGroup
                    select new
                    {
                        FacultyId = facultyGroup.FirstOrDefault().f.Id,
                        FacultyName = facultyGroup.FirstOrDefault().f.Name,
                        HaveContributions = facultyGroup.Any()
                    };

        var facultyHashmap = await query.ToListAsync();

        if (facultyHashmap is null || facultyHashmap.Any(x => x.HaveContributions == false))
        {
            _logger.LogInformation("All contribution have already been graded.");
            return;
        }

        var coordinators = await _userManager.GetUsersInRoleAsync(Roles.Coordinator);

        foreach (var coordinator in coordinators)
        {
            var facultyId = coordinator.FacultyId;

            if (!facultyHashmap.Any(x => x.FacultyId == facultyId && x.HaveContributions == true))
            {
                continue;
            }

            var url = $"{_configuration["ApplicationSettings:FrontendUrl"]}/mc/un-commented-contribution";

            var mail = new MailRequest
            {
                ToEmail = coordinator.Email,
                Subject = "REMINDER TO GRADE CONTRIBUTION",
                Body = $@"<html>
                        <body>
                            <h2>Coordinator Review Reminder</h2>
                            <p><strong>REMINDER:</strong> You have pending contributions in your faculty that require your attention.</p>
                            <p>As the coordinator for this faculty, we kindly request that you log in to the system at your earliest convenience to review these submissions. Timely assessment of contributions ensures that students receive feedback promptly and helps maintain the efficiency of the review process.</p>
                            <p>To review pending contributions, go to this link {url}</p>
                        </body>
                        </html>"
            };

            await _emailService.SendEmailAsync(mail);
        }

        _logger.LogInformation($"----- Email has been sent to all coordinator successfully ----- {_dateTimeProvider.UtcNow}");
    }
}
