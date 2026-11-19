namespace Temple.Application.Autenticacao;

public sealed record LoginRequest(
    string Email,
    string Senha);
