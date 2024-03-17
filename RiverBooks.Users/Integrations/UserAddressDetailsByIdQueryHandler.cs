﻿using Ardalis.Result;
using MediatR;
using RiverBooks.Users.Contracts;

namespace RiverBooks.Users.Integrations;

internal sealed class UserAddressDetailsByIdQueryHandler(IReadOnlyUserStreetAddressRepository addressRepository) 
    : IRequestHandler<UserAddressDetailsByIdQuery, Result<UserAddressDetails>>
{
    public async Task<Result<UserAddressDetails>> Handle(UserAddressDetailsByIdQuery request, 
        CancellationToken token = default)
    {
        var address = await addressRepository.GetById(request.AddressId, token);

        if (address is null)
        {
            return Result.NotFound();
        }

        var userId = Guid.Parse(address.UserId);

        var details = new UserAddressDetails(userId,
            address.Id,
            address.StreetAddress.Street1,
            address.StreetAddress.Street2,
            address.StreetAddress.City,
            address.StreetAddress.State,
            address.StreetAddress.PostalCode,
            address.StreetAddress.Country);

        return details;
    }

}