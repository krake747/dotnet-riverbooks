using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Riverbooks.SharedKernel.Behaviours;

namespace Riverbooks.SharedKernel;

public static class SharedKernelExtensions
{
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
        services.AddMediatRBehaviours();
        return services;
    }

    private static IServiceCollection AddMediatRBehaviours(this IServiceCollection services)
    {
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssemblyContaining(typeof(ISharedKernelMarker));
            // Order matters for chain of responsibility
            x.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            x.AddOpenBehavior(typeof(FluentValidationBehaviour<,>));
        });

        return services;
    }

    public static IServiceCollection AddFluentValidationValidators<TMarker>(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(TMarker));
        return services;
    }
}