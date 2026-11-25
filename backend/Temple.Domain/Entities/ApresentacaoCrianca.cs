using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class ApresentacaoCrianca
{
    public const int TamanhoMaximoNome = 150;
    public const int TamanhoMaximoOficiante = 150;
    public const int IdadeMaxima = 12;

    public Guid Id { get; private set; }
    public Guid IgrejaId { get; private set; }
    public string NomeDaCrianca { get; private set; }
    public DateOnly DataNascimentoDaCrianca { get; private set; }
    public string? NomeDoPai { get; private set; }
    public string? NomeDaMae { get; private set; }
    public DateOnly DataApresentacao { get; private set; }
    public string Oficiante { get; private set; }

    // Usado pelo EF Core ao ler do banco e pelo Registrar, depois de validar tudo.
    private ApresentacaoCrianca(
        Guid id,
        Guid igrejaId,
        string nomeDaCrianca,
        DateOnly dataNascimentoDaCrianca,
        string? nomeDoPai,
        string? nomeDaMae,
        DateOnly dataApresentacao,
        string oficiante)
    {
        Id = id;
        IgrejaId = igrejaId;
        NomeDaCrianca = nomeDaCrianca;
        DataNascimentoDaCrianca = dataNascimentoDaCrianca;
        NomeDoPai = nomeDoPai;
        NomeDaMae = nomeDaMae;
        DataApresentacao = dataApresentacao;
        Oficiante = oficiante;
    }

    public static ApresentacaoCrianca Registrar(
        Igreja igreja,
        string nomeDaCrianca,
        DateOnly dataNascimentoDaCrianca,
        string? nomeDoPai,
        string? nomeDaMae,
        DateOnly dataApresentacao,
        string oficiante,
        DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(igreja);

        if (!igreja.Ativa)
        {
            throw new DomainException("Não é possível registrar uma apresentação em uma igreja inativa.");
        }

        string nome = ValidarTexto(nomeDaCrianca, "nome da criança", TamanhoMaximoNome);
        string oficianteValidado = ValidarTexto(oficiante, "oficiante", TamanhoMaximoOficiante);
        string? pai = LimparNomeOpcional(nomeDoPai, "nome do pai");
        string? mae = LimparNomeOpcional(nomeDaMae, "nome da mãe");
        ValidarPais(pai, mae);
        ValidarDatas(dataNascimentoDaCrianca, dataApresentacao, agora);

        return new ApresentacaoCrianca(
            Guid.NewGuid(),
            igreja.Id,
            nome,
            dataNascimentoDaCrianca,
            pai,
            mae,
            dataApresentacao,
            oficianteValidado);
    }

    public void Corrigir(
        string nomeDaCrianca,
        DateOnly dataNascimentoDaCrianca,
        string? nomeDoPai,
        string? nomeDaMae,
        DateOnly dataApresentacao,
        string oficiante,
        bool temCertificadoEmitido,
        DateTimeOffset agora)
    {
        if (temCertificadoEmitido)
        {
            throw new DomainException("A apresentação já tem certificado emitido e não pode mais ser corrigida.");
        }

        // Valida tudo antes de alterar: se algo falhar, o registro continua como estava.
        string nome = ValidarTexto(nomeDaCrianca, "nome da criança", TamanhoMaximoNome);
        string oficianteValidado = ValidarTexto(oficiante, "oficiante", TamanhoMaximoOficiante);
        string? pai = LimparNomeOpcional(nomeDoPai, "nome do pai");
        string? mae = LimparNomeOpcional(nomeDaMae, "nome da mãe");
        ValidarPais(pai, mae);
        ValidarDatas(dataNascimentoDaCrianca, dataApresentacao, agora);

        NomeDaCrianca = nome;
        DataNascimentoDaCrianca = dataNascimentoDaCrianca;
        NomeDoPai = pai;
        NomeDaMae = mae;
        DataApresentacao = dataApresentacao;
        Oficiante = oficianteValidado;
    }

    private static void ValidarPais(string? nomeDoPai, string? nomeDaMae)
    {
        if (nomeDoPai is null && nomeDaMae is null)
        {
            throw new DomainException("Informe o nome do pai, o da mãe ou os dois.");
        }
    }

    private static void ValidarDatas(DateOnly dataNascimentoDaCrianca, DateOnly dataApresentacao, DateTimeOffset agora)
    {
        DateOnly hoje = DateOnly.FromDateTime(agora.DateTime);

        if (dataApresentacao > hoje)
        {
            throw new DomainException("A data da apresentação não pode estar no futuro.");
        }

        if (dataApresentacao < dataNascimentoDaCrianca)
        {
            throw new DomainException("A data da apresentação não pode ser anterior ao nascimento da criança.");
        }

        if (CalcularIdade(dataNascimentoDaCrianca, dataApresentacao) >= IdadeMaxima)
        {
            throw new DomainException($"Na data da apresentação, a criança deve ter menos de {IdadeMaxima} anos.");
        }
    }

    private static int CalcularIdade(DateOnly dataNascimento, DateOnly dataReferencia)
    {
        int idade = dataReferencia.Year - dataNascimento.Year;

        if (dataReferencia < dataNascimento.AddYears(idade))
        {
            idade--;
        }

        return idade;
    }

    private static string ValidarTexto(string valor, string nomeDoCampo, int tamanhoMaximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DomainException($"O {nomeDoCampo} é obrigatório.");
        }

        string valorLimpo = valor.Trim();

        if (valorLimpo.Length > tamanhoMaximo)
        {
            throw new DomainException($"O {nomeDoCampo} deve ter no máximo {tamanhoMaximo} caracteres.");
        }

        return valorLimpo;
    }

    private static string? LimparNomeOpcional(string? valor, string nomeDoCampo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        return ValidarTexto(valor, nomeDoCampo, TamanhoMaximoNome);
    }
}