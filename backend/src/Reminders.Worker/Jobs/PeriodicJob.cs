namespace Reminders.Worker.Jobs;

public abstract class PeriodicJob : BackgroundService
{
    private static readonly TimeSpan MinInterval = TimeSpan.FromMilliseconds(1);
    private static readonly TimeSpan MaxInterval = TimeSpan.FromMilliseconds(uint.MaxValue - 1);

    private readonly TimeSpan interval;
    private readonly TimeProvider timeProvider;
    private readonly ILogger logger;

    protected PeriodicJob(TimeSpan interval, TimeProvider timeProvider, ILogger logger)
    {
        if (interval < MinInterval || interval > MaxInterval)
            throw new ArgumentOutOfRangeException(
                nameof(interval),
                interval,
                $"Интервал должен быть от {MinInterval} до {MaxInterval}."
            );

        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(logger);

        this.interval = interval;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(
                        exception,
                        "Ошибка при выполнении прохода фоновой задачи {JobName}",
                        GetType().Name
                    );
                }

                await Task.Delay(interval, timeProvider, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Хост запросил остановку
        }
    }

    protected abstract Task RunOnceAsync(CancellationToken cancellationToken);
}
