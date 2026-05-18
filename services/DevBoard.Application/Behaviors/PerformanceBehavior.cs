using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DevBoard.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int ThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var timer = Stopwatch.StartNew();

        var response = await next();

        timer.Stop();

        if (timer.ElapsedMilliseconds > ThresholdMs)
            logger.LogWarning(
                "Slow request detected: {RequestName} took {ElapsedMs}ms",
                typeof(TRequest).Name,
                timer.ElapsedMilliseconds
            );

        return response;
    }
}