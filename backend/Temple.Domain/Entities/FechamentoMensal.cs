using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class FechamentoMensal
{
    public const int TamanhoMaximoNomeDoAutor = 150;
    public const int TamanhoMaximoMotivoDaReabertura = 500;

    public Guid Id { get; private set; }
    public Guid IgrejaId { get; private set; }
    public int Ano { get; private set; }
    public int Mes { get; private set; }

    // A foto dos totais no momento do fechamento.
    public decimal SaldoAnterior { get; private set; }
    public decimal TotalEntradas { get; private set; }
    public decimal TotalSaidas { get; private set; }
    public decimal SaldoFinal { get; private set; }

    public Guid FechadoPorId { get; private set; }
    public string FechadoPorNome { get; private set; }
    public DateTimeOffset FechadoEm { get; private set; }

    // Os quatro ficam nulos enquanto o fechamento não for reaberto.
    public string? MotivoDaReabertura { get; private set; }
    public Guid? ReabertoPorId { get; private set; }
    public string? ReabertoPorNome { get; private set; }
    public DateTimeOffset? ReabertoEm { get; private set; }

    public bool Reaberto => ReabertoEm is not null;

    public DateOnly PrimeiroDia => new(Ano, Mes, 1);

    public DateOnly UltimoDia => PrimeiroDia.AddMonths(1).AddDays(-1);

    // Construtor usado apenas pelo EF Core ao ler do banco.
    private FechamentoMensal(
        Guid id,
        Guid igrejaId,
        int ano,
        int mes,
        decimal saldoAnterior,
        decimal totalEntradas,
        decimal totalSaidas,
        decimal saldoFinal,
        Guid fechadoPorId,
        string fechadoPorNome,
        DateTimeOffset fechadoEm,
        string? motivoDaReabertura,
        Guid? reabertoPorId,
        string? reabertoPorNome,
        DateTimeOffset? reabertoEm)
    {
        Id = id;
        IgrejaId = igrejaId;
        Ano = ano;
        Mes = mes;
        SaldoAnterior = saldoAnterior;
        TotalEntradas = totalEntradas;
        TotalSaidas = totalSaidas;
        SaldoFinal = saldoFinal;
        FechadoPorId = fechadoPorId;
        FechadoPorNome = fechadoPorNome;
        FechadoEm = fechadoEm;
        MotivoDaReabertura = motivoDaReabertura;
        ReabertoPorId = reabertoPorId;
        ReabertoPorNome = reabertoPorNome;
        ReabertoEm = reabertoEm;
    }

    // Usado pela fábrica Fechar, que já validou o mês e a ordem.
    private FechamentoMensal(
        Guid igrejaId,
        int ano,
        int mes,
        decimal saldoAnterior,
        decimal totalEntradas,
        decimal totalSaidas,
        Guid fechadoPorId,
        string fechadoPorNome,
        DateTimeOffset agora)
    {
        Id = Guid.NewGuid();
        IgrejaId = igrejaId;
        Ano = ano;
        Mes = mes;
        SaldoAnterior = saldoAnterior;
        TotalEntradas = totalEntradas;
        TotalSaidas = totalSaidas;
        SaldoFinal = saldoAnterior + totalEntradas - totalSaidas;
        FechadoPorId = fechadoPorId;
        FechadoPorNome = ValidarNomeDoAutor(fechadoPorNome, "quem fechou o mês");
        FechadoEm = agora;
    }

    // ultimoFechado: o último fechamento ativo da igreja (nulo se não houver).
    // dataDoPrimeiroLancamento: a data do lançamento mais antigo da igreja (nulo se não houver).
    public static FechamentoMensal Fechar(
        Igreja igreja,
        int ano,
        int mes,
        FechamentoMensal? ultimoFechado,
        DateOnly? dataDoPrimeiroLancamento,
        decimal saldoAnterior,
        decimal totalEntradas,
        decimal totalSaidas,
        Guid fechadoPorId,
        string fechadoPorNome,
        DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(igreja);

        if (ano < 1 || ano > 9999 || mes < 1 || mes > 12)
        {
            throw new DomainException("Mês ou ano inválido.");
        }

        DateOnly primeiroDia = new(ano, mes, 1);
        DateOnly hoje = DateOnly.FromDateTime(agora.DateTime);

        if (primeiroDia.AddMonths(1) > hoje)
        {
            throw new DomainException("Só é possível fechar um mês que já terminou.");
        }

        DateOnly proximoAFechar = ProximoAFechar(igreja, ultimoFechado, dataDoPrimeiroLancamento);

        if (primeiroDia != proximoAFechar)
        {
            throw new DomainException($"Os meses são fechados em ordem: o próximo a fechar é {proximoAFechar.Month:D2}/{proximoAFechar.Year}.");
        }

        return new FechamentoMensal(
            igreja.Id, ano, mes,
            saldoAnterior, totalEntradas, totalSaidas,
            fechadoPorId, fechadoPorNome, agora);
    }

    // Só o último mês fechado pode ser reaberto, para o saldo anterior dos meses fechados nunca mudar.
    public void Reabrir(bool ehOUltimoFechado, string motivo, Guid reabertoPorId, string reabertoPorNome, DateTimeOffset agora)
    {
        if (Reaberto)
        {
            throw new DomainException("Este fechamento já foi reaberto.");
        }

        if (!ehOUltimoFechado)
        {
            throw new DomainException("Só o último mês fechado pode ser reaberto.");
        }

        // Valida tudo antes de alterar: se algo falhar, o fechamento continua como estava.
        string motivoValidado = ValidarMotivo(motivo);
        string nomeValidado = ValidarNomeDoAutor(reabertoPorNome, "quem reabriu o mês");

        MotivoDaReabertura = motivoValidado;
        ReabertoPorId = reabertoPorId;
        ReabertoPorNome = nomeValidado;
        ReabertoEm = agora;
    }

    private static DateOnly ProximoAFechar(Igreja igreja, FechamentoMensal? ultimoFechado, DateOnly? dataDoPrimeiroLancamento)
    {
        if (ultimoFechado is not null)
        {
            if (ultimoFechado.IgrejaId != igreja.Id || ultimoFechado.Reaberto)
            {
                throw new ArgumentException("O último fechamento precisa ser um fechamento ativo da mesma igreja.", nameof(ultimoFechado));
            }

            return ultimoFechado.PrimeiroDia.AddMonths(1);
        }

        // Sem nenhum mês fechado, o primeiro a fechar é o mês do lançamento mais antigo.
        if (dataDoPrimeiroLancamento is not DateOnly primeiraData)
        {
            throw new DomainException("A igreja ainda não tem lançamentos para fechar.");
        }

        return new DateOnly(primeiraData.Year, primeiraData.Month, 1);
    }

    private static string ValidarMotivo(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
        {
            throw new DomainException("O motivo da reabertura é obrigatório.");
        }

        string motivoLimpo = motivo.Trim();

        if (motivoLimpo.Length > TamanhoMaximoMotivoDaReabertura)
        {
            throw new DomainException($"O motivo da reabertura deve ter no máximo {TamanhoMaximoMotivoDaReabertura} caracteres.");
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