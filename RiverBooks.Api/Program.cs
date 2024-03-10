using FastEndpoints;
using RiverBooks.Books;
using RiverBooks.Users;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger.Information("Starting RiverBooks web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddFastEndpoints();

builder.Services.AddBooksModule(builder.Configuration, Log.Logger);
builder.Services.AddUsersModule(builder.Configuration, Log.Logger);

var app = builder.Build();

app.UseFastEndpoints();

app.MapGet("/", () => "Hello RiverBooks Api!");

app.Run();