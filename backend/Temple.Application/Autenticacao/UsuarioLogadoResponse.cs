namespace Temple.Application.Autenticacao;

public sealed record UsuarioLogadoResponse(
    Guid Id,
    string Nome,
    string Email);
