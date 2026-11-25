namespace Temple.Application.Apresentacoes;

public sealed record ApresentacaoResponse(
    Guid Id,
    Guid IgrejaId,
    string NomeDaCrianca,
    DateOnly DataNascimentoDaCrianca,
    string? NomeDoPai,
    string? NomeDaMae,
    DateOnly DataApresentacao,
    string Oficiante);