using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Reminders.Worker.Jobs;

namespace Reminders.Worker.Tests;

public class PeriodicJobTests
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task StartsImmediatelyAndWaitsForThePassAndIntervalBeforeStartingAgain()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var timeProvider = new ObservableTimeProvider();
        var firstPassStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finishFirstPass = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondPassStarted = new TaskCompletionSource<DateTimeOffset>(TaskCreationOptions.RunContinuationsAsynchronously);

        using var job = new TestJob(
            Interval, 
            timeProvider, 
            token =>
            {
                if (firstPassStarted.TrySetResult())
                    return finishFirstPass.Task.WaitAsync(token);

                secondPassStarted.TrySetResult(timeProvider.GetUtcNow());
                return Task.CompletedTask;
            }
        );

        await job.StartAsync(cancellationToken);

        try
        {
            await firstPassStarted.Task.WaitAsync(TestTimeout, cancellationToken);

            timeProvider.Advance(Interval * 2);
            Assert.False(secondPassStarted.Task.IsCompleted);
            Assert.False(timeProvider.FirstDelay.Task.IsCompleted);

            var firstPassFinishedAt = timeProvider.GetUtcNow();
            finishFirstPass.SetResult();
            Assert.Equal(Interval, await timeProvider.FirstDelay.Task.WaitAsync(TestTimeout, cancellationToken));

            var lastMillisecond = TimeSpan.FromMilliseconds(1);
            timeProvider.Advance(Interval - lastMillisecond);
            Assert.False(secondPassStarted.Task.IsCompleted);

            timeProvider.Advance(lastMillisecond);
            Assert.Equal(
                firstPassFinishedAt + Interval,
                await secondPassStarted.Task.WaitAsync(TestTimeout, cancellationToken)
            );
        }
        finally
        {
            await job.StopAsync(cancellationToken).WaitAsync(TestTimeout, cancellationToken);
        }
    }

    [Fact]
    public async Task LogsPassFailureAndStartsAnotherPassAfterTheInterval()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var timeProvider = new ObservableTimeProvider();
        var logger = Substitute.For<ILogger>();
        var failure = new InvalidOperationException("Ошибка прохода для проверки логирования.");
        var secondPassStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var passesStarted = 0;

        using var job = new TestJob(
            Interval, 
            timeProvider, 
            _ =>
            {
                if (Interlocked.Increment(ref passesStarted) == 1)
                    throw failure;

                secondPassStarted.TrySetResult();
                return Task.CompletedTask;
            }, 
            logger);

        await job.StartAsync(cancellationToken);

        try
        {
            Assert.Equal(Interval, await timeProvider.FirstDelay.Task.WaitAsync(TestTimeout, cancellationToken));
            var errorLog = Assert.Single(logger.ReceivedCalls(), call =>
                call.GetMethodInfo().Name == nameof(ILogger.Log) &&
                call.GetArguments()[0] is LogLevel.Error);
            Assert.Same(failure, errorLog.GetArguments()[3]);
            Assert.False(secondPassStarted.Task.IsCompleted);

            timeProvider.Advance(Interval);
            await secondPassStarted.Task.WaitAsync(TestTimeout, cancellationToken);
        }
        finally
        {
            await job.StopAsync(cancellationToken).WaitAsync(TestTimeout, cancellationToken);
        }
    }

    [Fact]
    public async Task StopAsyncCancelsAnActivePass()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var passStarted = new TaskCompletionSource<CancellationToken>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var job = new TestJob(
            Interval, 
            new FakeTimeProvider(), 
            token =>
            {
                passStarted.SetResult(token);
                return Task.Delay(Timeout.InfiniteTimeSpan, token);
            }
        );

        await job.StartAsync(cancellationToken);

        CancellationToken passToken;
        try
        {
            passToken = await passStarted.Task.WaitAsync(TestTimeout, cancellationToken);
        }
        finally
        {
            await job.StopAsync(cancellationToken).WaitAsync(TestTimeout, cancellationToken);
        }

        Assert.True(passToken.IsCancellationRequested);
        Assert.Equal(TaskStatus.RanToCompletion, job.ExecuteTask!.Status);
    }

    [Fact]
    public async Task StopAsyncCancelsTheDelayBetweenPasses()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var timeProvider = new ObservableTimeProvider();
        using var job = new TestJob(
            Interval, 
            timeProvider, 
            _ => Task.CompletedTask
        );

        await job.StartAsync(cancellationToken);

        try
        {
            await timeProvider.FirstDelay.Task.WaitAsync(TestTimeout, cancellationToken);
        }
        finally
        {
            await job.StopAsync(cancellationToken).WaitAsync(TestTimeout, cancellationToken);
        }

        Assert.Equal(TaskStatus.RanToCompletion, job.ExecuteTask!.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    public void ConstructorRejectsAnIntervalOutsideTheSupportedRange(int days)
    {
        var interval = TimeSpan.FromDays(days);
        Assert.Throws<ArgumentOutOfRangeException>("interval", () =>
            new TestJob(
                interval, 
                new FakeTimeProvider(), 
                _ => Task.CompletedTask)
            );
    }

    private sealed class TestJob(
        TimeSpan interval,
        TimeProvider timeProvider,
        Func<CancellationToken, Task> runOnceAsync,
        ILogger? logger = null
    )
        : PeriodicJob(interval, timeProvider, logger ?? NullLogger.Instance)
    {
        protected override Task RunOnceAsync(CancellationToken cancellationToken) =>
            runOnceAsync(cancellationToken);
    }

    private sealed class ObservableTimeProvider : FakeTimeProvider
    {
        public TaskCompletionSource<TimeSpan> FirstDelay { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            var timer = base.CreateTimer(callback, state, dueTime, period);
            FirstDelay.TrySetResult(dueTime);
            return timer;
        }
    }
}
