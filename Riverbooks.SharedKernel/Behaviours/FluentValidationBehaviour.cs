using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Riverbooks.SharedKernel.Behaviours;

public sealed class FluentValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken token = default)
    {
        if (validators.Any() is false)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, token)));
        var resultErrors = validationResults.SelectMany(r => r.AsErrors()).ToList();
        var failures = validationResults.SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count is 0)
        {
            return await next();
        }

        switch (typeof(TResponse).IsGenericType)
        {
            case true when typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>):
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var invalidMethod = typeof(Result<>)
                    .MakeGenericType(resultType)
                    .GetMethod(nameof(Result<int>.Invalid), [typeof(List<ValidationError>)]);

                if (invalidMethod is not null)
                {
                    return (TResponse)invalidMethod.Invoke(null, [resultErrors])!;
                }

                break;
            }
            default:
            {
                if (typeof(TResponse) != typeof(Result))
                {
                    throw new ValidationException(failures);
                }

                return (TResponse)(object)Result.Invalid(resultErrors);
            }
        }

        return await next();
    }
}