using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using MongoDB.Driver;

namespace RiverBooks.EmailSending;

public static class EmailSendingModuleExtensions
{
    public static IServiceCollection AddEmailSendingModule(this IServiceCollection services, 
        ConfigurationManager config, ILogger logger)
    {
        services.AddMongoDb(config);
        
        services.AddTransient<ISendEmail, MimeKitEmailSender>();
        services.AddTransient<IOutboxService, MongoDbOutboxService>();
        
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(IEmailSendingModuleMarker)));
        
        logger.Information("{Module} module services registered", "Email Sending");
        
        return services;
    }
    
    private static IServiceCollection AddMongoDb(this IServiceCollection services, ConfigurationManager config)
    {
        var s = config.GetSection("MongoDB");
        services.Configure<MongoDbSettings>(config.GetSection("MongoDB"));
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings!.DatabaseName);
        });

        services.AddTransient(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<EmailOutboxEntity>("EmailOutboxEntityCollection");
        });
        
        return services;
    }
}