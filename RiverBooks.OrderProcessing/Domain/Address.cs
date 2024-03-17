﻿namespace RiverBooks.OrderProcessing.Domain;

public sealed record Address(
    string Street1,
    string Street2,
    string City,
    string State,
    string PostalCode,
    string Country);