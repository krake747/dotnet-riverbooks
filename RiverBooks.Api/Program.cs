using RiverBooks.Books;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBookServices();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapBookEndpoints();

app.Run();