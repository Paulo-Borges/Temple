using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class Batismo
{
    public const int TamanhoMaximoOficiante = 150;
    public const int TamanhoMaximoLocal = 150;

    public Guid Id { get; private set; }
    public Guid MembroId { get; private set; }
    public DateOnly DataBatismo { get; private set; }
    public string Oficiante { get; private set; }
    public string? Local { get; private set; }

    // Usado pelo EF Core ao ler do banco e pelo Registrar, depois de validar tudo.
    private Batismo(Guid id, Guid membroId, DateOnly dataBatismo, string oficiante, string? local)
    {
        Id = id;
        MembroId = membroId;
        DataBatismo = dataBatismo;
        Oficiante = oficiante;
        Local = local;
    }

    public static Batismo Registrar(Membro membro, DateOnly dataBatismo, string oficiante, string? local, DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(membro);

        if (!membro.Ativo)
        {
            throw new DomainException("Não é possível registrar o batismo de um membro desativado.");
        }

        return new Batismo(
            Guid.NewGuid(),
            membro.Id,
            ValidarData(dataBatismo, membro, agora),
            ValidarOficiante(oficiante),
            LimparLocal(local));
    }

    public void Corrigir(Membro membro, DateOnly dataBatismo, string oficiante, string? local, bool temCertificadoEmitido, DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(membro);

        if (membro.Id != MembroId)
        {
            throw new InvalidOperationException("O membro informado não é o membro do batismo.");
        }

        if (temCertificadoEmitido)
        {
            throw new DomainException("O batismo já tem certificado emitido e não pode mais ser corrigido.");
        }

        // Valida tudo antes de alterar: se algo falhar, o registro continua como estava.
        DateOnly data = ValidarData(dataBatismo, membro, agora);
        string oficianteValidado = ValidarOficiante(oficiante);
        string? localLimpo = LimparLocal(local);

        DataBatismo = data;
        Oficiante = oficianteValidado;
        Local = localLimpo;
    }

    private static DateOnly ValidarData(DateOnly dataBatismo, Membro membro, DateTimeOffset agora)
    {
        DateOnly hoje = DateOnly.FromDateTime(agora.DateTime);

        if (dataBatismo > hoje)
        {
            throw new DomainException("A data do batismo não pode estar no futuro.");
        }

        if (dataBatismo < membro.DataNascimento)
        {
            throw new DomainException("A data do batismo não pode ser anterior ao nascimento do membro.");
        }

        return dataBatismo;
    }

    private static string ValidarOficiante(string oficiante)
    {
        if (string.IsNullOrWhiteSpace(oficiante))
        {
            throw new DomainException("O oficiante é obrigatório.");
        }

        string oficianteLimpo = oficiante.Trim();

        if (oficianteLimpo.Length > TamanhoMaximoOficiante)
        {
            throw new DomainException($"O oficiante deve ter no máximo {TamanhoMaximoOficiante} caracteres.");
        }

        return oficianteLimpo;
    }

    private static string? LimparLocal(string? local)
    {
        if (string.IsNullOrWhiteSpace(local))
        {
            return null;
        }

        string localLimpo = local.Trim();

        if (localLimpo.Length > TamanhoMaximoLocal)
        {
            throw new DomainException($"O local deve ter no máximo {TamanhoMaximoLocal} caracteres.");
        }

        return localLimpo;
    }
}