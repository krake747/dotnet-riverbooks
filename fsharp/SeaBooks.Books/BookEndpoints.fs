namespace SeaBooks.Books

open System

type BookDto = { Id: Guid; Title: string; Author: string }

module BookModule =

    let listBooks () : BookDto seq =
        seq {
            {
                Id = Guid.NewGuid()
                Title = "The Fellowship of the Ring"
                Author = "J.R.R. Tolkien"
            }

            {
                Id = Guid.NewGuid()
                Title = "The Two Towers"
                Author = "J.R.R. Tolkien"
            }

            {
                Id = Guid.NewGuid()
                Title = "The Return of the Ring"
                Author = "J.R.R. Tolkien"
            }
        }

    type BookService() =
        member _.ListBooks() = listBooks ()

module BookEndpoints =

    open System.Runtime.CompilerServices
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.AspNetCore.Http
    open Giraffe
    open Giraffe.EndpointRouting
    open BookModule

    [<Extension>]
    type IServiceCollectionExtensions =
        [<Extension>]
        static member inline AddBookService(services: IServiceCollection) : IServiceCollection =
            services.AddScoped<BookService>(fun sp -> BookService())

    let listBooksHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            let bookService = ctx.GetService<BookService>()
            bookService.ListBooks() |> ctx.WriteJsonAsync

    let bookEndpoints = [ GET [ route "/books" listBooksHandler ] ]
