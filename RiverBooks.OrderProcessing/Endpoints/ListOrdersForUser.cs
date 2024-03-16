using System.Security.Claims;
using Ardalis.Result;
using FastEndpoints;
using MediatR;

namespace RiverBooks.OrderProcessing.Endpoints;

public sealed class ListOrdersForUserResponse
{
    public IEnumerable<OrderSummary> Orders { get; init; } = [];
}

public sealed class OrderSummary
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset DateShipped { get; set; }
}

internal sealed record ListOrdersForUserQuery(string EmailAddress) : IRequest<Result<IEnumerable<OrderSummary>>>;

internal sealed class ListOrdersForUserHandler(IOrderRepository orderRepository) 
    : IRequestHandler<ListOrdersForUserQuery, Result<IEnumerable<OrderSummary>>>
{
    public async Task<Result<IEnumerable<OrderSummary>>> Handle(ListOrdersForUserQuery request,
        CancellationToken token = default)

    {
        // TODO: Lookup UserId for EmailAddress, Filter by User
        var orders = await orderRepository.ListAsync(token);
        
        var summaries = orders.Select(o => new OrderSummary
        {
            OrderId = o.Id,
            UserId = o.UserId,
            DateCreated = o.DateCreated,
            Total = o.OrderItems.Sum(x => x.UnitPrice)
        });

        return summaries.ToList();
    }
}

internal sealed class ListOrdersForUser(ISender mediator) : EndpointWithoutRequest<ListOrdersForUserResponse>
{
    public override void Configure()
    {
        Get("/orders");
        Claims("EmailAddress");
    }

    public override async Task HandleAsync(CancellationToken token)
    {
        var emailAddress = User.FindFirstValue("EmailAddress");

        var query = new ListOrdersForUserQuery(emailAddress!);

        var result = await mediator.Send(query, token);

        if (result.Status is ResultStatus.Unauthorized)
        {
            await SendUnauthorizedAsync(token);
        }
        else
        {
            var response = new ListOrdersForUserResponse
            {
                Orders = result.Value.Select(o => new OrderSummary
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    Total = o.Total,
                    DateCreated = o.DateCreated,
                    DateShipped = o.DateShipped
                })
            };
            
            await SendOkAsync(response, token);
        }

    }
}