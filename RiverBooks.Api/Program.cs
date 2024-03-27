using FastEndpoints;
using FastEndpoints.Security;
using RiverBooks.Books;
using RiverBooks.EmailSending;
using RiverBooks.OrderProcessing;
using Riverbooks.Reporting;
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

builder.Services.AddSharedKernel();
builder.Services.AddBooksModule(builder.Configuration, Log.Logger);
builder.Services.AddEmailSendingModule(builder.Configuration, Log.Logger);
builder.Services.AddOrderProcessingModule(builder.Configuration, Log.Logger);
builder.Services.AddReportingModule(builder.Configuration, Log.Logger);
builder.Services.AddUsersModule(builder.Configuration, Log.Logger);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

app.MapGet("/", () => "Hello RiverBooks Api!");

app.Run();