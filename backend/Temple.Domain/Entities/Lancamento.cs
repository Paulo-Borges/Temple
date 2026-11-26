using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class Lancamento
{
    public const int CasasDecimais = 2;
    public const int TamanhoMaximoDescricao = 200;
    public const int TamanhoMaximoNomeDoAutor = 150;
    public const int TamanhoMaximoMotivoDoEstorno = 500;

    // O maior valor que cabe numa coluna decimal(18,2): 16 dígitos inteiros e 2 decimais.
    public const decimal ValorMaximo = 9_999_999_999_999_999.99m;

    public Guid Id { get; private set; }
    public Guid IgrejaId { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public DateOnly Data { get; private set; }
    public decimal Valor { get; private set; }
    public FormaPagamento FormaPagamento { get; private set; }
    public string? Descricao { get; private set; }

    public Guid RegistradoPorId { get; private set; }
    public string RegistradoPorNome { get; private set; }
    public DateTimeOffset RegistradoEm { get; private set; }

    // Os quatro ficam nulos enquanto o lançamento não for estornado.
    public string? MotivoDoEstorno { get; private set; }
    public Guid? EstornadoPorId { get; private set; }
    public string? EstornadoPorNome { get; private set; }
    public DateTimeOffset? EstornadoEm { get; private set; }

    public NaturezaLancamento Natureza => Tipo == TipoLancamento.Despesa ? NaturezaLancamento.Saida : NaturezaLancamento.Entrada;

    public bool Estornado => EstornadoEm is not null;

    // Construtor usado apenas pelo EF Core ao ler do banco.
    private Lancamento(
        Guid id,
        Guid igrejaId,
        TipoLancamento tipo,
        DateOnly data,
        decimal valor,
        FormaPagamento formaPagamento,
        string? descricao,
        Guid registradoPorId,
        string registradoPorNome,
        DateTimeOffset registradoEm,
        string? motivoDoEstorno,
        Guid? estornadoPorId,
        string? estornadoPorNome,
        DateTimeOffset? estornadoEm)
    {
        Id = id;
        IgrejaId = igrejaId;
        Tipo = tipo;
        Data = data;
        Valor = valor;
        FormaPagamento = formaPagamento;
        Descricao = descricao;
        RegistradoPorId = registradoPorId;
        RegistradoPorNome = registradoPorNome;
        RegistradoEm = registradoEm;
        MotivoDoEstorno = motivoDoEstorno;
        EstornadoPorId = estornadoPorId;
        EstornadoPorNome = estornadoPorNome;
        EstornadoEm = estornadoEm;
    }

    // Usado pelas fábricas de cada tipo (Aula 5B.2), que acrescentam as regras próprias.
    private Lancamento(
        Igreja igreja,
        TipoLancamento tipo,
        DateOnly data,
        decimal valor,
        FormaPagamento formaPagamento,
        string? descricao,
        bool descricaoObrigatoria,
        Guid registradoPorId,
        string registradoPorNome,
        DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(igreja);

        if (!igreja.Ativa)
        {
            throw new DomainException("Não é possível registrar lançamento em uma igreja inativa.");
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new DomainException("Tipo de lançamento inválido.");
        }

        Id = Guid.NewGuid();
        IgrejaId = igreja.Id;
        Tipo = tipo;
        Data = ValidarData(data, agora);
        Valor = ValidarValor(valor);
        FormaPagamento = ValidarFormaPagamento(formaPagamento);
        Descricao = ValidarDescricao(descricao, descricaoObrigatoria);
        RegistradoPorId = registradoPorId;
        RegistradoPorNome = ValidarNomeDoAutor(registradoPorNome, "quem registrou o lançamento");
        RegistradoEm = agora;
    }

    // Saldo: a entrada soma e a saída subtrai. O lançamento estornado não entra no saldo.
    public decimal ValorNoSaldo()
    {
        if (Estornado)
        {
            return 0m;
        }

        return Natureza == NaturezaLancamento.Entrada ? Valor : -Valor;
    }

    public void Estornar(string motivo, Guid estornadoPorId, string estornadoPorNome, DateTimeOffset agora)
    {
        if (Estornado)
        {
            throw new DomainException("Este lançamento já foi estornado.");
        }

        // Valida tudo antes de alterar: se algo falhar, o lançamento continua como estava.
        string motivoValidado = ValidarMotivo(motivo);
        string nomeValidado = ValidarNomeDoAutor(estornadoPorNome, "quem estornou o lançamento");

        MotivoDoEstorno = motivoValidado;
        EstornadoPorId = estornadoPorId;
        EstornadoPorNome = nomeValidado;
        EstornadoEm = agora;
    }

    private static DateOnly ValidarData(DateOnly data, DateTimeOffset agora)
    {
        DateOnly hoje = DateOnly.FromDateTime(agora.DateTime);

        if (data > hoje)
        {
            throw new DomainException("A data do lançamento não pode estar no futuro.");
        }

        return data;
    }

    private static decimal ValidarValor(decimal valor)
    {
        if (valor <= 0m)
        {
            throw new DomainException("O valor deve ser maior que zero.");
        }

        if (valor > ValorMaximo)
        {
            throw new DomainException("O valor é grande demais.");
        }

        if (decimal.Round(valor, CasasDecimais) != valor)
        {
            throw new DomainException($"O valor deve ter no máximo {CasasDecimais} casas decimais.");
        }

        return valor;
    }

    private static FormaPagamento ValidarFormaPagamento(FormaPagamento formaPagamento)
    {
        if (!Enum.IsDefined(formaPagamento))
        {
            throw new DomainException("Forma de pagamento inválida.");
        }

        return formaPagamento;
    }

    private static string? ValidarDescricao(string? descricao, bool obrigatoria)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            if (obrigatoria)
            {
                throw new DomainException("A descrição é obrigatória para este tipo de lançamento.");
            }

            return null;
        }

        string descricaoLimpa = descricao.Trim();

        if (descricaoLimpa.Length > TamanhoMaximoDescricao)
        {
            throw new DomainException($"A descrição deve ter no máximo {TamanhoMaximoDescricao} caracteres.");
        }

        return descricaoLimpa;
    }

    private static string ValidarMotivo(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
        {
            throw new DomainException("O motivo do estorno é obrigatório.");
        }

        string motivoLimpo = motivo.Trim();

        if (motivoLimpo.Length > TamanhoMaximoMotivoDoEstorno)
        {
            throw new DomainException($"O motivo do estorno deve ter no máximo {TamanhoMaximoMotivoDoEstorno} caracteres.");
        }

        return motivoLimpo;
    }

    private static string ValidarNomeDoAutor(string nome, string papel)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException($"O nome de {papel} é obrigatório.");
        }

        string nomeLimpo = nome.Trim();

        if (nomeLimpo.Length > TamanhoMaximoNomeDoAutor)
        {
            throw new DomainException($"O nome de {papel} deve ter no máximo {TamanhoMaximoNomeDoAutor} caracteres.");
        }

        return nomeLimpo;
    }
}