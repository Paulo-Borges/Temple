using Temple.Application.Comum;

namespace Temple.Application.Igrejas;

public sealed record IgrejaResponse(
    Guid Id,
    string Nome,
    string Cnpj,
    string Telefone,
    DateOnly DataFundacao,
    bool Ativa,
    bool EhMatriz,
    Guid? IgrejaMatrizId,
    EnderecoDto Endereco);
