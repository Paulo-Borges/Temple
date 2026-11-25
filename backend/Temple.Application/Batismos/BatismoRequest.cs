namespace Temple.Application.Batismos;

public sealed record BatismoRequest(
    DateOnly DataBatismo,
    string Oficiante,
    string? Local = null);