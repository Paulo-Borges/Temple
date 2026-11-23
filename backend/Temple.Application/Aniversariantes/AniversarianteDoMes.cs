using Temple.Domain.Enums;

namespace Temple.Application.Aniversariantes;

public sealed record AniversarianteDoMes(
    Guid Id,
    int Matricula,
    string Nome,
    DateOnly DataNascimento,
    Guid IgrejaId,
    string NomeDaIgreja,
    GrupoMembro? Grupo);