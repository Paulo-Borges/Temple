namespace Temple.Application.Autenticacao;

public sealed record LoginResponse(
    string Token,
    DateTimeOffset ExpiraEm);
