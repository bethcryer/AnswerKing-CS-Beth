﻿namespace Answer.King.Api.RequestModels;

public record Category
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
}