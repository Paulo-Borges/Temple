using Temple.Application.Apresentacoes;
using Temple.Application.Autenticacao;
using Temple.Application.Batismos;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Application.Membros;
using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Application.Certificados;

public sealed class CertificadoService(
    ICertificadoRepository certificadoRepository,
    IMembroRepository membroRepository,
    IBatismoRepository batismoRepository,
    IApresentacaoRepository apresentacaoRepository,
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

        UsuarioAutenticado emissor = await ObterEmissorAsync(cancellationToken);
        CertificadoEmitido certificado = CertificadoEmitido.DePromocao(membro, historico, emissor.Id, emissor.Nome, timeProvider.GetLocalNow());

        await RegistrarAsync(certificado, cancellationToken);

        return await GerarPdfDePromocaoAsync(certificado, membro, historico, cancellationToken);
    }

    public async Task<CertificadoPdf> EmitirDeBatismoAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Membro membro = await ObterMembroAsync(membroId, cancellationToken);
        Batismo batismo = await ObterBatismoAsync(membro.Id, cancellationToken);

        if (await certificadoRepository.ExisteParaBatismoAsync(batismo.Id, cancellationToken))
        {
            throw new ConflitoException("Este batismo já tem certificado emitido. Use a 2ª via pelo número do certificado.");
        }

        UsuarioAutenticado emissor = await ObterEmissorAsync(cancellationToken);
        CertificadoEmitido certificado = CertificadoEmitido.DeBatismo(membro, batismo, emissor.Id, emissor.Nome, timeProvider.GetLocalNow());

        await RegistrarAsync(certificado, cancellationToken);

        return await GerarPdfDeBatismoAsync(certificado, membro, batismo, cancellationToken);
    }

    public async Task<CertificadoPdf> EmitirDeApresentacaoAsync(Guid apresentacaoId, CancellationToken cancellationToken)
    {
        ApresentacaoCrianca apresentacao = await ObterApresentacaoAsync(apresentacaoId, cancellationToken);

        if (await certificadoRepository.ExisteParaApresentacaoAsync(apresentacao.Id, cancellationToken))
        {
            throw new ConflitoException("Esta apresentação já tem certificado emitido. Use a 2ª via pelo número do certificado.");
        }

        UsuarioAutenticado emissor = await ObterEmissorAsync(cancellationToken);
        CertificadoEmitido certificado = CertificadoEmitido.DeApresentacao(apresentacao, emissor.Id, emissor.Nome, timeProvider.GetLocalNow());

        await RegistrarAsync(certificado, cancellationToken);

        return await GerarPdfDeApresentacaoAsync(certificado, apresentacao, cancellationToken);
    }

    public async Task<CertificadoPdf> ObterSegundaViaAsync(int numero, CancellationToken cancellationToken)
    {
        CertificadoEmitido? certificado = await certificadoRepository.ObterPorNumeroAsync(numero, cancellationToken);

        if (certificado is null)
        {
            throw NaoEncontrado(numero);
        }

        return certificado.Tipo switch
        {
            TipoCertificado.PromocaoCargo => await SegundaViaDePromocaoAsync(certificado, cancellationToken),
            TipoCertificado.Batismo => await SegundaViaDeBatismoAsync(certificado, cancellationToken),
            TipoCertificado.ApresentacaoCrianca => await SegundaViaDeApresentacaoAsync(certificado, cancellationToken),
            _ => throw new InvalidOperationException($"O certificado nº {numero} tem um tipo desconhecido.")
        };
    }

    private async Task<CertificadoPdf> SegundaViaDePromocaoAsync(CertificadoEmitido certificado, CancellationToken cancellationToken)
    {
        Membro membro = await ObterMembroDoCertificadoAsync(certificado, cancellationToken);

        if (certificado.HistoricoCargoId is not Guid historicoCargoId)
        {
            throw new InvalidOperationException($"O certificado nº {certificado.Numero} não está ligado a uma promoção.");
        }

        HistoricoCargo historico = ObterPromocao(membro, historicoCargoId);

        return await GerarPdfDePromocaoAsync(certificado, membro, historico, cancellationToken);
    }

    private async Task<CertificadoPdf> SegundaViaDeBatismoAsync(CertificadoEmitido certificado, CancellationToken cancellationToken)
    {
        Membro membro = await ObterMembroDoCertificadoAsync(certificado, cancellationToken);
        Batismo batismo = await ObterBatismoAsync(membro.Id, cancellationToken);

        if (certificado.BatismoId != batismo.Id)
        {
            throw new InvalidOperationException($"O certificado nº {certificado.Numero} não está ligado ao batismo do membro.");
        }

        return await GerarPdfDeBatismoAsync(certificado, membro, batismo, cancellationToken);
    }

    private async Task<CertificadoPdf> SegundaViaDeApresentacaoAsync(CertificadoEmitido certificado, CancellationToken cancellationToken)
    {
        if (certificado.ApresentacaoCriancaId is not Guid apresentacaoId)
        {
            throw new InvalidOperationException($"O certificado nº {certificado.Numero} não está ligado a uma apresentação.");
        }

        ApresentacaoCrianca? apresentacao = await apresentacaoRepository.ObterPorIdAsync(apresentacaoId, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (apresentacao is null || !escopo.AlcancaIgreja(apresentacao.IgrejaId))
        {
            throw NaoEncontrado(certificado.Numero);
        }

        return await GerarPdfDeApresentacaoAsync(certificado, apresentacao, cancellationToken);
    }

    private Task<CertificadoPdf> GerarPdfDePromocaoAsync(CertificadoEmitido certificado, Membro membro, HistoricoCargo historico, CancellationToken cancellationToken)
    {
        string texto = $"Certificamos que {membro.Nome} recebeu o cargo de "
            + $"{TextosDoCertificado.NomeDoCargo(historico.CargoNovo)} em "
            + $"{TextosDoCertificado.DataPorExtenso(historico.DataPromocao)}, "
            + "conforme os registros desta igreja.";

        return GerarPdfAsync(certificado, membro.IgrejaId, "Certificado de Promoção de Cargo", texto, cancellationToken);
    }

    private Task<CertificadoPdf> GerarPdfDeBatismoAsync(CertificadoEmitido certificado, Membro membro, Batismo batismo, CancellationToken cancellationToken)
    {
        string local = batismo.Local is string valor ? $", em {valor}" : string.Empty;

        string texto = $"Certificamos que {membro.Nome} recebeu o batismo em "
            + $"{TextosDoCertificado.DataPorExtenso(batismo.DataBatismo)}{local}, "
            + $"ministrado por {batismo.Oficiante}, conforme os registros desta igreja.";

        return GerarPdfAsync(certificado, membro.IgrejaId, "Certificado de Batismo", texto, cancellationToken);
    }

    private Task<CertificadoPdf> GerarPdfDeApresentacaoAsync(CertificadoEmitido certificado, ApresentacaoCrianca apresentacao, CancellationToken cancellationToken)
    {
        string texto = $"Certificamos que a criança {apresentacao.NomeDaCrianca}, nascida em "
            + $"{TextosDoCertificado.DataPorExtenso(apresentacao.DataNascimentoDaCrianca)}, "
            + $"{TextosDoCertificado.Filiacao(apresentacao.NomeDoPai, apresentacao.NomeDaMae)}, "
            + $"foi apresentada ao Senhor em {TextosDoCertificado.DataPorExtenso(apresentacao.DataApresentacao)}, "
            + $"por {apresentacao.Oficiante}, nesta igreja.";

        return GerarPdfAsync(certificado, apresentacao.IgrejaId, "Certificado de Apresentação de Criança", texto, cancellationToken);
    }

    private async Task<CertificadoPdf> GerarPdfAsync(CertificadoEmitido certificado, Guid igrejaId, string titulo, string texto, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(igrejaId, cancellationToken);

        if (igreja is null)
        {
            throw new InvalidOperationException($"A igreja '{igrejaId}' do certificado não existe.");
        }

        DateOnly dataDeEmissao = DateOnly.FromDateTime(certificado.EmitidoEm.DateTime);

        DadosDoCertificado dados = new(
            certificado.Numero,
            igreja.Nome,
            titulo,
            texto,
            $"{igreja.Endereco.Cidade} ({igreja.Endereco.Uf}), {TextosDoCertificado.DataPorExtenso(dataDeEmissao)}");

        byte[] conteudo = geradorDePdf.Gerar(dados);

        return new CertificadoPdf(certificado.Numero, TextosDoCertificado.NomeDoArquivo(certificado.Numero), conteudo);
    }

    private async Task RegistrarAsync(CertificadoEmitido certificado, CancellationToken cancellationToken)
    {
        certificadoRepository.Adicionar(certificado);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    private async Task<UsuarioAutenticado> ObterEmissorAsync(CancellationToken cancellationToken)
    {
        UsuarioAutenticado? emissor = await servicoDeIdentidade.ObterPorIdAsync(usuarioAtual.Id, cancellationToken);

        if (emissor is null)
        {
            throw new NaoAutorizadoException("O usuário do token não existe mais.");
        }

        return emissor;
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

    private async Task<Membro> ObterMembroDoCertificadoAsync(CertificadoEmitido certificado, CancellationToken cancellationToken)
    {
        if (certificado.MembroId is not Guid membroId)
        {
            throw new InvalidOperationException($"O certificado nº {certificado.Numero} não está ligado a um membro.");
        }

        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (membro is null || !escopo.AlcancaMembro(membro))
        {
            throw NaoEncontrado(certificado.Numero);
        }

        return membro;
    }

    private async Task<Batismo> ObterBatismoAsync(Guid membroId, CancellationToken cancellationToken)
    {
        Batismo? batismo = await batismoRepository.ObterPorMembroAsync(membroId, cancellationToken);

        if (batismo is null)
        {
            throw new NaoEncontradoException("O membro não tem batismo registrado.");
        }

        return batismo;
    }

    private async Task<ApresentacaoCrianca> ObterApresentacaoAsync(Guid id, CancellationToken cancellationToken)
    {
        ApresentacaoCrianca? apresentacao = await apresentacaoRepository.ObterPorIdAsync(id, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (apresentacao is null || !escopo.AlcancaIgreja(apresentacao.IgrejaId))
        {
            throw new NaoEncontradoException($"Apresentação '{id}' não encontrada.");
        }

        return apresentacao;
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

    private static NaoEncontradoException NaoEncontrado(int numero)
    {
        return new NaoEncontradoException($"Certificado nº {numero} não encontrado.");
    }
}