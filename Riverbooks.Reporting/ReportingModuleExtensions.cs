using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Riverbooks.Reporting;

public static class ReportingModuleExtensions
{
    public static IServiceCollection AddReportingModule(this IServiceCollection services,
        ConfigurationManager config, ILogger logger)
    {
        var connectionString = config.GetConnectionString("Reporting");

        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IReportingModuleMarker)));
        
        logger.Information("{Module} module services registered", "Reporting");

        return services;
    }
}