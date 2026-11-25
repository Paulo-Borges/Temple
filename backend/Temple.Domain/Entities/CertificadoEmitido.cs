using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class CertificadoEmitido
{
    public const int TamanhoMaximoNomeDoEmissor = 150;

    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public TipoCertificado Tipo { get; private set; }

    // Nulo só na apresentação de criança, que é um registro avulso (a criança não é membro).
    public Guid? MembroId { get; private set; }

    // Cada certificado aponta para um único registro, conforme o tipo.
    public Guid? HistoricoCargoId { get; private set; }
    public Guid? BatismoId { get; private set; }
    public Guid? ApresentacaoCriancaId { get; private set; }

    public Guid EmitidoPorId { get; private set; }
    public string EmitidoPorNome { get; private set; }
    public DateTimeOffset EmitidoEm { get; private set; }

    // Construtor usado apenas pelo EF Core ao ler do banco.
    private CertificadoEmitido(
        Guid id,
        int numero,
        TipoCertificado tipo,
        Guid? membroId,
        Guid? historicoCargoId,
        Guid? batismoId,
        Guid? apresentacaoCriancaId,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset emitidoEm)
    {
        Id = id;
        Numero = numero;
        Tipo = tipo;
        MembroId = membroId;
        HistoricoCargoId = historicoCargoId;
        BatismoId = batismoId;
        ApresentacaoCriancaId = apresentacaoCriancaId;
        EmitidoPorId = emitidoPorId;
        EmitidoPorNome = emitidoPorNome;
        EmitidoEm = emitidoEm;
    }

    private CertificadoEmitido(
        TipoCertificado tipo,
        Guid? membroId,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset emitidoEm)
    {
        Id = Guid.NewGuid();
        Tipo = tipo;
        MembroId = membroId;
        EmitidoPorId = emitidoPorId;
        EmitidoPorNome = ValidarNomeDoEmissor(emitidoPorNome);
        EmitidoEm = emitidoEm;
    }

    public static CertificadoEmitido DePromocao(
        Membro membro,
        HistoricoCargo historico,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(membro);
        ArgumentNullException.ThrowIfNull(historico);

        if (historico.MembroId != membro.Id)
        {
            throw new InvalidOperationException("A promoção informada não pertence ao membro.");
        }

        ValidarMembroAtivo(membro);

        if (historico.EhRebaixamento)
        {
            throw new DomainException("Não há certificado de promoção para um rebaixamento de cargo.");
        }

        return new CertificadoEmitido(TipoCertificado.PromocaoCargo, membro.Id, emitidoPorId, emitidoPorNome, agora)
        {
            HistoricoCargoId = historico.Id
        };
    }

    public static CertificadoEmitido DeBatismo(
        Membro membro,
        Batismo batismo,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(membro);
        ArgumentNullException.ThrowIfNull(batismo);

        if (batismo.MembroId != membro.Id)
        {
            throw new InvalidOperationException("O batismo informado não pertence ao membro.");
        }

        ValidarMembroAtivo(membro);

        return new CertificadoEmitido(TipoCertificado.Batismo, membro.Id, emitidoPorId, emitidoPorNome, agora)
        {
            BatismoId = batismo.Id
        };
    }

    public static CertificadoEmitido DeApresentacao(
        ApresentacaoCrianca apresentacao,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset agora)
    {
        ArgumentNullException.ThrowIfNull(apresentacao);

        return new CertificadoEmitido(TipoCertificado.ApresentacaoCrianca, membroId: null, emitidoPorId, emitidoPorNome, agora)
        {
            ApresentacaoCriancaId = apresentacao.Id
        };
    }

    private static void ValidarMembroAtivo(Membro membro)
    {
        if (!membro.Ativo)
        {
            throw new DomainException("Não é possível emitir certificado para um membro desativado.");
        }
    }

    private static string ValidarNomeDoEmissor(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome de quem emitiu o certificado é obrigatório.");
        }

        string nomeLimpo = nome.Trim();

        if (nomeLimpo.Length > TamanhoMaximoNomeDoEmissor)
        {
            throw new DomainException($"O nome de quem emitiu o certificado deve ter no máximo {TamanhoMaximoNomeDoEmissor} caracteres.");
        }

        return nomeLimpo;
    }
}