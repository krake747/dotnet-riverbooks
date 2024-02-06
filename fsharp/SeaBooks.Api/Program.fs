open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open Giraffe.EndpointRouting

open SeaBooks.Books.BookEndpoints
open SeaBooks.Books.BookModule

let endpoints = [ GET [ route "/" (text "Hello World from Giraffe") ] ]

let notFoundHandler = "Not Found" |> text |> RequestErrors.notFound

let configureApp (appBuilder: IApplicationBuilder) =
    appBuilder
        .UseRouting()
        // .UseGiraffe(endpoints)
        .UseGiraffe(bookEndpoints)
        .UseGiraffe(notFoundHandler)

let configureServices (services: IServiceCollection) =
    services
        .AddRouting()
        .AddBookService()
        .AddGiraffe()
        .AddSingleton<BookService>(BookService())
    |> ignore

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    configureServices builder.Services

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore

    configureApp app
    app.Run()

    0 // Exit code
