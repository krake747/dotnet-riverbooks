using System.Diagnostics;
using System.Reflection;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Riverbooks.SharedKernel.Behaviours;

public sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken token = default)
    {
        _ = Guard.Against.Null(request);
        var name = request.GetType().Name;
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Handling {RequestName}", name);

            // Reflection! Could be a performance concern
            var myType = request.GetType();
            var props = new List<PropertyInfo>(myType.GetProperties());
            foreach (var prop in props)
            {
                var propValue = prop.GetValue(request, null);
                logger.LogInformation("Property {Property} : {Value}", prop.Name, propValue);
            }
        }

        try
        {
            var sw = Stopwatch.StartNew();

            var response = await next();

            logger.LogInformation("Handled {RequestName} with {Response} in {ElapsedMilliseconds} ms",
                name, response, sw.ElapsedMilliseconds);

            sw.Stop();
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Command {Command} processing failed", name);
            throw;
        }
    }
}