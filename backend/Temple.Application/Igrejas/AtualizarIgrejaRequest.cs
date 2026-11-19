using Temple.Application.Comum;

namespace Temple.Application.Igrejas;

public sealed record AtualizarIgrejaRequest(
    string Nome,
    string Telefone,
    EnderecoDto Endereco);

