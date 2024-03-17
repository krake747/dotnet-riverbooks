using FastEndpoints;
using FastEndpoints.Security;
using RiverBooks.Books;
using RiverBooks.OrderProcessing;
using Riverbooks.SharedKernel;
using RiverBooks.Users;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger.Information("Starting RiverBooks web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Auth:JwtSecret"]);
builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints();

builder.Services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

builder.Services.AddBooksModule(builder.Configuration, Log.Logger);
builder.Services.AddUsersModule(builder.Configuration, Log.Logger);
builder.Services.AddOrderProcessingModule(builder.Configuration, Log.Logger);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.MapGet("/", () => "Hello RiverBooks Api!");

app.Run();