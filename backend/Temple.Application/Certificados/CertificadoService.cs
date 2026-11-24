using Temple.Application.Autenticacao;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Application.Membros;
using Temple.Domain.Entities;

namespace Temple.Application.Certificados;

public sealed class CertificadoService(
    ICertificadoRepository certificadoRepository,
    IMembroRepository membroRepository,
    IIgrejaRepository igrejaRepository,
    IUnitOfWork unitOfWork,
    IServicoDeIdentidade servicoDeIdentidade,
    ProvedorDeEscopo provedorDeEscopo,
    IUsuarioAtual usuarioAtual,
    IGeradorDeCertificadoPdf geradorDePdf,
    TimeProvider timeProvider)
{
    public async Task<CertificadoPdf> EmitirDePromocaoAsync(Guid membroId, Guid historicoCargoId, CancellationToken cancellationToken)
    {
        Membro membro = await ObterMembroAsync(membroId, cancellationToken);
        HistoricoCargo historico = ObterPromocao(membro, historicoCargoId);

        if (await certificadoRepository.ExisteParaPromocaoAsync(historico.Id, cancellationToken))
        {
            throw new ConflitoException("Esta promoção já tem certificado emitido. Use a 2ª via pelo número do certificado.");
        }

        UsuarioAutenticado? emissor = await servicoDeIdentidade.ObterPorIdAsync(usuarioAtual.Id, cancellationToken);

        if (emissor is null)
        {
            throw new NaoAutorizadoException("O usuário do token não existe mais.");
        }

        CertificadoEmitido certificado = CertificadoEmitido.DePromocao(membro, historico, emissor.Id, emissor.Nome, timeProvider.GetLocalNow());

        certificadoRepository.Adicionar(certificado);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return await GerarPdfAsync(certificado, membro, historico, cancellationToken);
    }

    public async Task<CertificadoPdf> ObterSegundaViaAsync(int numero, CancellationToken cancellationToken)
    {
        CertificadoEmitido? certificado = await certificadoRepository.ObterPorNumeroAsync(numero, cancellationToken);
        Membro? membro = null;

        if (certificado is not null)
        {
            membro = await membroRepository.ObterPorIdAsync(certificado.MembroId, cancellationToken);
        }

        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (certificado is null || membro is null || !escopo.AlcancaMembro(membro))
        {
            throw new NaoEncontradoException($"Certificado nº {numero} não encontrado.");
        }

        if (certificado.HistoricoCargoId is not Guid historicoCargoId)
        {
            throw new InvalidOperationException($"O certificado nº {numero} não está ligado a uma promoção.");
        }

        HistoricoCargo historico = ObterPromocao(membro, historicoCargoId);

        return await GerarPdfAsync(certificado, membro, historico, cancellationToken);
    }

    private async Task<CertificadoPdf> GerarPdfAsync(CertificadoEmitido certificado, Membro membro, HistoricoCargo historico, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(membro.IgrejaId, cancellationToken);

        if (igreja is null)
        {
            throw new InvalidOperationException($"A igreja '{membro.IgrejaId}' do membro não existe.");
        }

        DateOnly dataDeEmissao = DateOnly.FromDateTime(certificado.EmitidoEm.DateTime);

        string texto = $"Certificamos que {membro.Nome} recebeu o cargo de "
            + $"{TextosDoCertificado.NomeDoCargo(historico.CargoNovo)} em "
            + $"{TextosDoCertificado.DataPorExtenso(historico.DataPromocao)}, "
            + "conforme os registros desta igreja.";

        DadosDoCertificado dados = new(
            certificado.Numero,
            igreja.Nome,
            "Certificado de Promoção de Cargo",
            texto,
            $"{igreja.Endereco.Cidade} ({igreja.Endereco.Uf}), {TextosDoCertificado.DataPorExtenso(dataDeEmissao)}");

        byte[] conteudo = geradorDePdf.Gerar(dados);

        return new CertificadoPdf(certificado.Numero, TextosDoCertificado.NomeDoArquivo(certificado.Numero), conteudo);
    }

    private async Task<Membro> ObterMembroAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro? membro = await membroRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (membro is null || !escopo.AlcancaMembro(membro))
        {
            throw new NaoEncontradoException($"Membro '{id}' não encontrado.");
        }

        return membro;
    }

    private static HistoricoCargo ObterPromocao(Membro membro, Guid historicoCargoId)
    {
        HistoricoCargo? historico = membro.HistoricoCargos.FirstOrDefault(item => item.Id == historicoCargoId);

        if (historico is null)
        {
            throw new NaoEncontradoException($"Promoção '{historicoCargoId}' não encontrada no histórico do membro.");
        }

        return historico;
    }
}