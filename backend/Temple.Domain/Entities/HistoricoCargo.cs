using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class HistoricoCargo
{
    public const int TamanhoMaximoObservacao = 500;
    public const int TamanhoMaximoNomeDoAutor = 150;

    public Guid Id { get; private set; }
    public Guid MembroId { get; private set; }
    public TipoCargo CargoAnterior { get; private set; }
    public TipoCargo CargoNovo { get; private set; }
    public DateOnly DataPromocao { get; private set; }
    public string? Observacao { get; private set; }

    // Os três são nulos só nos registros gravados antes da Aula 5.2, quando ainda não eram guardados.
    public Guid? PromovidoPorId { get; private set; }
    public string? PromovidoPorNome { get; private set; }
    public DateTimeOffset? RegistradoEm { get; private set; }

    public bool EhRebaixamento => CargoNovo < CargoAnterior;

    // Construtor usado apenas pelo EF Core ao ler do banco.
    private HistoricoCargo(
        Guid id,
        Guid membroId,
        TipoCargo cargoAnterior,
        TipoCargo cargoNovo,
        DateOnly dataPromocao,
        string? observacao,
        Guid? promovidoPorId,
        string? promovidoPorNome,
        DateTimeOffset? registradoEm)
    {
        Id = id;
        MembroId = membroId;
        CargoAnterior = cargoAnterior;
        CargoNovo = cargoNovo;
        DataPromocao = dataPromocao;
        Observacao = observacao;
        PromovidoPorId = promovidoPorId;
        PromovidoPorNome = promovidoPorNome;
        RegistradoEm = registradoEm;
    }

    internal HistoricoCargo(
        Guid membroId,
        TipoCargo cargoAnterior,
        TipoCargo cargoNovo,
        DateOnly dataPromocao,
        string? observacao,
        Guid promovidoPorId,
        string promovidoPorNome,
        DateTimeOffset registradoEm)
    {
        Id = Guid.NewGuid();
        MembroId = membroId;
        CargoAnterior = cargoAnterior;
        CargoNovo = cargoNovo;
        DataPromocao = dataPromocao;
        Observacao = LimparObservacao(observacao);
        PromovidoPorId = promovidoPorId;
        PromovidoPorNome = ValidarNomeDoAutor(promovidoPorNome);
        RegistradoEm = registradoEm;
    }

    private static string? LimparObservacao(string? observacao)
    {
        if (string.IsNullOrWhiteSpace(observacao))
        {
            return null;
        }

        string observacaoLimpa = observacao.Trim();

        if (observacaoLimpa.Length > TamanhoMaximoObservacao)
        {
            throw new DomainException($"A observação deve ter no máximo {TamanhoMaximoObservacao} caracteres.");
        }

        return observacaoLimpa;
    }

    private static string ValidarNomeDoAutor(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome de quem fez a promoção é obrigatório.");
        }

        string nomeLimpo = nome.Trim();

        if (nomeLimpo.Length > TamanhoMaximoNomeDoAutor)
        {
            throw new DomainException($"O nome de quem fez a promoção deve ter no máximo {TamanhoMaximoNomeDoAutor} caracteres.");
        }

        return nomeLimpo;
    }
}
