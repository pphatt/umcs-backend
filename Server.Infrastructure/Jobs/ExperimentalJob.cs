using Microsoft.Extensions.Logging;
using Quartz;
using Server.Application.Common.Interfaces.Services;

namespace Server.Infrastructure.Jobs;

public class ExperimentalJob : IJob
{
    private readonly ILogger<ExperimentalJob> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ExperimentalJob(ILogger<ExperimentalJob> logger, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{_dateTimeProvider.UtcNow}");

        return Task.CompletedTask;
    }
}
