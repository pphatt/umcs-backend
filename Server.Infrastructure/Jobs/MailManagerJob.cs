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

[DisallowConcurrentExecution]
public class MailManagerJob : IJob
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailManagerJob> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MailManagerJob(
        AppDbContext context,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<MailManagerJob> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"----- Initialize send email to all managers ----- {_dateTimeProvider.UtcNow}");

        var haveAnyContribution = await _context.Contributions
            .AnyAsync(x => x.IsCoordinatorCommented == false && x.Status == ContributionStatus.Pending);

        var managers = await _userManager.GetUsersInRoleAsync(Roles.Manager);

        var ungradedContributionUrl = _configuration["ApplicationSettings:UngradedContribution"];

        if (haveAnyContribution)
        {
            foreach (var manager in managers)
            {
                var mail = new MailRequest
                {
                    ToEmail = manager.Email,
                    Subject = "GRADING CONTRIBUTION",
                    Body = $"There are some contributions which are not yet graded or do not have comments. To view more detail, see this link: {ungradedContributionUrl}"
                };

                await _emailService.SendEmailAsync(mail);
            }
        }
        else
        {
            foreach (var manager in managers)
            {
                var mail = new MailRequest
                {
                    ToEmail = manager.Email,
                    Subject = "GRADING CONTRIBUTION",
                    Body = "All contribution has been comment by coordinators"
                };

                await _emailService.SendEmailAsync(mail);
            }
        }

        _logger.LogInformation($"----- Email has been sent to all managers successfully ----- {_dateTimeProvider.UtcNow}");
    }
}
