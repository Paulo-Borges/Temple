namespace Temple.Application.Batismos;

public sealed record BatismoResponse(
    Guid Id,
    Guid MembroId,
    DateOnly DataBatismo,
    string Oficiante,
    string? Local);