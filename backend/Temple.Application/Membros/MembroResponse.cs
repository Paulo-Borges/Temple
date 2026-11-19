using Temple.Application.Comum;
using Temple.Domain.Enums;

namespace Temple.Application.Membros;

public sealed record MembroResponse(
    Guid Id,
    int Matricula,
    string Nome,
    DateOnly DataNascimento,
    EnderecoDto Endereco,
    Guid IgrejaId,
    TipoCargo CargoAtual,
    GrupoMembro? Grupo,
    bool Ativo);
