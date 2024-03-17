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
        // // Get the assembly containing the specified type
        // var assembly = typeof(T).GetTypeInfo().Assembly;
        //
        // // Find all validator types in the assembly
        // var validatorTypes = assembly.GetTypes()
        //     .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && 
        //                                            i.GetGenericTypeDefinition() == typeof(IValidator<>)))
        //     .ToList();
        //
        // // Register each validator with its implemented interfaces
        // foreach (var validatorType in validatorTypes)
        // {
        //     var implementedInterfaces = validatorType.GetInterfaces()
        //         .Where(i => i.IsGenericType && 
        //                     i.GetGenericTypeDefinition() == typeof(IValidator<>));
        //
        //     foreach (var implementedInterface in implementedInterfaces)
        //     {
        //         services.AddTransient(implementedInterface, validatorType);
        //     }
        // }

        services.AddValidatorsFromAssemblyContaining(typeof(TMarker));
        return services;
    }
}