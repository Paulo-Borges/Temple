using Temple.Domain.Enums;

namespace Temple.Application.Aniversariantes;

public sealed record AniversarianteResponse(
    Guid Id,
    int Matricula,
    string Nome,
    DateOnly DataNascimento,
    int Dia,
    int IdadeQueCompleta,
    Guid IgrejaId,
    string NomeDaIgreja,
    GrupoMembro? Grupo);