namespace Temple.Application.Apresentacoes;

public sealed record AtualizarApresentacaoRequest(
    string NomeDaCrianca,
    DateOnly DataNascimentoDaCrianca,
    DateOnly DataApresentacao,
    string Oficiante,
    string? NomeDoPai = null,
    string? NomeDaMae = null);