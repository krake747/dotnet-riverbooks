using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RiverBooks.EmailSending;

public static class EmailSendingModuleExtensions
{
    public static IServiceCollection AddEmailSendingModule(this IServiceCollection services, 
        ConfigurationManager config, ILogger logger)
    {
        services.AddTransient<ISendEmail, MimeKitEmailSender>();
        
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IEmailSendingModuleMarker)));
        
        logger.Information("{Module} module services registered", "Email Sending");
        
        return services;
    }
}