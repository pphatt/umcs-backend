using Microsoft.Extensions.Options;
using Quartz;

namespace Server.Infrastructure.Jobs.JobSetup;

public class JobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        // Experimental Job (5 seconds).
        var experimentalJobKey = JobKey.Create(nameof(ExperimentalJob));

        options
            .AddJob<ExperimentalJob>(JobBuilder => JobBuilder.WithIdentity(experimentalJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(experimentalJobKey)
                    //.WithCronSchedule("*/1 * * * *")
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(5).RepeatForever()));

        // Mail Manager Job (run at 00:00:00 every day).
        var mailManagerJobKey = JobKey.Create(nameof(MailManagerJob));

        options
            .AddJob<MailManagerJob>(jobBuilder => jobBuilder.WithIdentity(mailManagerJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(mailManagerJobKey)
                    .WithCronSchedule("0 0 0 * * ?"));

        // Mail Coordinator (run at 00:00:00 every day).
        var mailCoordinatorJobKey = JobKey.Create(nameof(MailCoordinatorJob));

        options
            .AddJob<MailCoordinatorJob>(jobBuilder => jobBuilder.WithIdentity(mailCoordinatorJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(mailCoordinatorJobKey)
                    .WithCronSchedule("0 0 0 * * ?"));

        // Notify Student About Closure Date (run at 00:00:00 every day).
        var notifyStudentAboutCloseDateJobKey = JobKey.Create(nameof(NotifyStudentAboutClosureDateJob));

        options
            .AddJob<NotifyStudentAboutClosureDateJob>(jobBuilder => jobBuilder.WithIdentity(notifyStudentAboutCloseDateJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(notifyStudentAboutCloseDateJobKey)
                    .WithCronSchedule("0 0 0 * * ?"));

        // Reject Contribution Job (10 seconds).
        var rejectContributionJobKey = JobKey.Create(nameof(RejectContributionJob));

        options
            .AddJob<RejectContributionJob>(jobBuilder => jobBuilder.WithIdentity(rejectContributionJobKey))
            .AddTrigger(trigger =>
                trigger
                    .ForJob(rejectContributionJobKey)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(10).RepeatForever()));
    }
}
