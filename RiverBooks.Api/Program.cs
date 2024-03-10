using FastEndpoints;
using RiverBooks.Books;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services.AddBooksModule(builder.Configuration);

var app = builder.Build();

app.UseFastEndpoints();

app.MapGet("/", () => "Hello RiverBooks Api!");

app.Run();