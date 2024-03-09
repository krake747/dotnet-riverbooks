using FastEndpoints;
using RiverBooks.Books;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();

builder.Services.AddBookServices();

var app = builder.Build();

app.UseFastEndpoints();

app.MapGet("/", () => "Hello World!");
app.MapBookEndpoints();

app.Run();