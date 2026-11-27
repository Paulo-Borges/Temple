using Temple.Application.Certificados;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Igrejas;
using Temple.Application.Lancamentos;
using Temple.Application.Membros;
using Temple.Domain.Entities;
using Temple.Domain.Enums;
using Temple.Domain.Exceptions;

namespace Temple.Application.Comprovantes;

public sealed class ComprovanteService(
    ILancamentoRepository lancamentoRepository,
    IIgrejaRepository igrejaRepository,
    IMembroRepository membroRepository,
    ProvedorDeEscopo provedorDeEscopo,
    IGeradorDeReciboPdf geradorDeRecibo)
{
    // Gerado na hora, a partir do lançamento: não há número nem registro de emissão.
    public async Task<ReciboPdf> GerarReciboDeDizimoAsync(Guid lancamentoId, CancellationToken cancellationToken)
    {
        Lancamento? lancamento = await lancamentoRepository.ObterPorIdAsync(lancamentoId, cancellationToken);
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (lancamento is null || !escopo.AlcancaIgreja(lancamento.IgrejaId))
        {
            throw new NaoEncontradoException($"Lançamento '{lancamentoId}' não encontrado.");
        }

        if (lancamento.Tipo != TipoLancamento.Dizimo)
        {
            throw new DomainException("O recibo é emitido só para dízimos.");
        }

        if (lancamento.Estornado)
        {
            throw new DomainException("Um dízimo estornado não tem recibo.");
        }

        // O dízimo sempre tem membro (Domain e CHECK do banco); se faltar, é erro de programação.
        if (lancamento.MembroId is not Guid membroId)
        {
            throw new InvalidOperationException($"O dízimo '{lancamento.Id}' está sem membro.");
        }

        Membro? membro = await membroRepository.ObterPorIdAsync(membroId, cancellationToken);
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(lancamento.IgrejaId, cancellationToken);

        if (membro is null || igreja is null)
        {
            throw new InvalidOperationException($"O dízimo '{lancamento.Id}' aponta para um membro ou uma igreja que não existe.");
        }

        string valor = TextosDoComprovante.ValorEmReais(lancamento.Valor);

        string texto = $"Recebemos de {membro.Nome}, matrícula {membro.Matricula}, a quantia de {valor} "
            + $"({ValorPorExtenso.EmReais(lancamento.Valor)}), referente ao dízimo, "
            + $"pago {TextosDoComprovante.FormaDePagamento(lancamento.FormaPagamento)}.";

        DadosDoRecibo dados = new(
            igreja.Nome,
            TextosDoComprovante.CnpjFormatado(igreja.Cnpj),
            valor,
            texto,
            lancamento.Descricao,
            $"{igreja.Endereco.Cidade}, {TextosDoCertificado.DataPorExtenso(lancamento.Data)}",
            $"Registrado por {lancamento.RegistradoPorNome} em {TextosDoComprovante.DataEHora(lancamento.RegistradoEm)}. "
                + $"Código do lançamento: {lancamento.Id}");

        byte[] conteudo = geradorDeRecibo.Gerar(dados);

        return new ReciboPdf(TextosDoComprovante.NomeDoArquivo(lancamento.Data, lancamento.Id), conteudo);
    }
}