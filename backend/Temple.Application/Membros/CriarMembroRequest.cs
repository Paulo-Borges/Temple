using Temple.Application.Comum;
using Temple.Domain.Enums;

namespace Temple.Application.Membros;

public sealed record CriarMembroRequest(
    string Nome,
    DateOnly DataNascimento,
    EnderecoDto Endereco,
    Guid IgrejaId,
    TipoCargo CargoAtual,
    GrupoMembro? Grupo);
