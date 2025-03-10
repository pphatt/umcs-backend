using Microsoft.Extensions.Configuration;
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

        // Mail Manager Job (24 hours).
        var mailManagerJobKey = JobKey.Create(nameof(MailManagerJob));

        options
            .AddJob<MailManagerJob>(jobBuilder => jobBuilder.WithIdentity(mailManagerJobKey))
            .AddTrigger(trigger => 
                trigger
                    .ForJob(mailManagerJobKey)
                    .WithSimpleSchedule(schedule => 
                        schedule.WithIntervalInMinutes(60).RepeatForever()));
    }
}
