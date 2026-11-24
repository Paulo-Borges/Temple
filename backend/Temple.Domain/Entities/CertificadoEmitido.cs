using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Domain.Entities;

public sealed class CertificadoEmitido
{
    public const int TamanhoMaximoNomeDoEmissor = 150;

    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public TipoCertificado Tipo { get; private set; }
    public Guid MembroId { get; private set; }

    // Preenchido só nos certificados de promoção de cargo.
    public Guid? HistoricoCargoId { get; private set; }

    public Guid EmitidoPorId { get; private set; }
    public string EmitidoPorNome { get; private set; }
    public DateTimeOffset EmitidoEm { get; private set; }

    // Construtor usado apenas pelo EF Core ao ler do banco.
    private CertificadoEmitido(
        Guid id,
        int numero,
        TipoCertificado tipo,
        Guid membroId,
        Guid? historicoCargoId,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset emitidoEm)
    {
        Id = id;
        Numero = numero;
        Tipo = tipo;
        MembroId = membroId;
        HistoricoCargoId = historicoCargoId;
        EmitidoPorId = emitidoPorId;
        EmitidoPorNome = emitidoPorNome;
        EmitidoEm = emitidoEm;
    }

    private CertificadoEmitido(
        TipoCertificado tipo,
        Guid membroId,
        Guid? historicoCargoId,
        Guid emitidoPorId,
        string emitidoPorNome,
        DateTimeOffset emitidoEm)
    {
        Id = Guid.NewGuid();
        Tipo = tipo;
        MembroId = membroId;
        HistoricoCargoId = historicoCargoId;
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

        if (!membro.Ativo)
        {
            throw new DomainException("Não é possível emitir certificado para um membro desativado.");
        }

        if (historico.EhRebaixamento)
        {
            throw new DomainException("Não há certificado de promoção para um rebaixamento de cargo.");
        }

        return new CertificadoEmitido(TipoCertificado.PromocaoCargo, membro.Id, historico.Id, emitidoPorId, emitidoPorNome, agora);
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