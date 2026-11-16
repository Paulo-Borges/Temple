using Temple.Application.Comum;

namespace Temple.Application.Igrejas;

public sealed record CriarIgrejaRequest(
    string Nome,
    string Cnpj,
    string Telefone,
    DateOnly DataFundacao,
    bool Ativa,
    EnderecoDto Endereco);
