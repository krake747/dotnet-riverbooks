using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using RiverBooks.Books.BookEndpoints;

namespace RiverBooks.Books.Tests.Integration.Endpoints;

public sealed class BookEndpointsTests(RiverBooksApiFactory factory)
    : IClassFixture<RiverBooksApiFactory>, IAsyncLifetime
{
    private readonly List<string> _createdIds = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var httpClient = factory.CreateClient();

        foreach (var createdId in _createdIds)
        {
            await httpClient.DeleteAsync($"books/{createdId}");
        }
    }


    [Fact]
    public async Task GetAllBooks_ShouldReturnAllBooks()
    {
        // Arrange
        var httpClient = factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync("books");
        var response = await result.Content.ReadFromJsonAsync<GetAllBooksResponse>();
        var books = response!.Books.ToList();

        // Assert
        using var _ = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        books.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetBookById_ShouldReturnBook()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        var id = Guid.Parse("A89F6CD7-4693-457B-9009-02205DBBFE45");
        var expected = new BookDto(id, "The Fellowship of the Ring", "J.R.R. Tolkien", 10.99m);
        var request = new GetBookByIdRequest { Id = id };

        // Act
        var result = await httpClient.GetAsync($"books/{request.Id}");
        var book = await result.Content.ReadFromJsonAsync<BookDto>();

        // Assert
        using var _ = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        book.Should().BeEquivalentTo(expected);
    }
}