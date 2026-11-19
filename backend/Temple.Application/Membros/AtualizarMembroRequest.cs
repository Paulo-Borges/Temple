using Temple.Application.Comum;
using Temple.Domain.Enums;

namespace Temple.Application.Membros;

public sealed record AtualizarMembroRequest(
    string Nome,
    EnderecoDto Endereco,
    GrupoMembro? Grupo);
