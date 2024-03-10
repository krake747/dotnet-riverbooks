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
    public async Task ReturnsBooksList()
    {
        // Arrange
        var httpClient = factory.CreateClient();
        
        // Act
        var result = await httpClient.GetAsync("books");
        var booksResponse = await result.Content.ReadFromJsonAsync<GetAllBooksResponse>();
        var books = booksResponse!.Books.ToList();
        
        // Assert
        using var _ = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        books.Should().NotBeEmpty()
            .And.HaveCount(3);
    }
}