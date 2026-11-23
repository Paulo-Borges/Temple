using FluentValidation;
using Temple.Application.Comum;
using Temple.Application.Comum.Acesso;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Domain.Entities;

namespace Temple.Application.Igrejas;

public sealed class IgrejaService(
    IIgrejaRepository igrejaRepository,
    IUnitOfWork unitOfWork,
    ProvedorDeEscopo provedorDeEscopo,
    IValidator<CriarIgrejaRequest> criarIgrejaValidator,
    IValidator<AtualizarIgrejaRequest> atualizarIgrejaValidator)
{
    public async Task<IgrejaResponse> CriarMatrizAsync(CriarIgrejaRequest request, CancellationToken cancellationToken)
    {
        await criarIgrejaValidator.ValidateAndThrowAsync(request, cancellationToken);

        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        if (!escopo.Total)
        {
            throw new AcessoNegadoException("Só a administração geral pode cadastrar uma nova igreja matriz.");
        }

        Igreja matriz = request.ParaMatriz();

        await GarantirCnpjDisponivelAsync(matriz.Cnpj, cancellationToken);

        igrejaRepository.Adicionar(matriz);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return matriz.ParaResponse();
    }

    public async Task<IgrejaResponse> CriarFilialAsync(Guid matrizId, CriarIgrejaRequest request, CancellationToken cancellationToken)
    {
        await criarIgrejaValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja matriz = await ObterEntidadeAsync(matrizId, cancellationToken);
        Igreja filial = request.ParaFilialDe(matriz);

        await GarantirCnpjDisponivelAsync(filial.Cnpj, cancellationToken);

        igrejaRepository.Adicionar(filial);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return filial.ParaResponse();
    }

    public async Task<IgrejaResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Igreja igreja = await ObterEntidadeAsync(id, cancellationToken);

        return igreja.ParaResponse();
    }

        public async Task<IReadOnlyList<IgrejaResponse>> ListarAsync(bool? ativa, CancellationToken cancellationToken)
    {
        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);
         IReadOnlyList<Igreja> igrejas = await igrejaRepository.ListarAsync(ativa, escopo, cancellationToken);

        return igrejas
            .Select(igreja => igreja.ParaResponse())
            .ToList();
    }

    public async Task<IReadOnlyList<IgrejaResponse>> ListarFiliaisAsync(Guid matrizId, CancellationToken cancellationToken)
    {
        Igreja matriz = await ObterEntidadeAsync(matrizId, cancellationToken);

        return matriz.Filiais
            .OrderBy(filial => filial.Nome)
            .Select(filial => filial.ParaResponse())
            .ToList();
    }

    public async Task<IgrejaResponse> AtualizarAsync(Guid id, AtualizarIgrejaRequest request, CancellationToken cancellationToken)
    {
        await atualizarIgrejaValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterEntidadeAsync(id, cancellationToken);

        igreja.AlterarNome(request.Nome);
        igreja.AlterarTelefone(request.Telefone);
        igreja.AlterarEndereco(request.Endereco.ParaEndereco());

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return igreja.ParaResponse();
    }

    public async Task AtivarAsync(Guid id, CancellationToken cancellationToken)
    {
        Igreja igreja = await ObterEntidadeAsync(id, cancellationToken);

        igreja.Ativar();

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task DesativarAsync(Guid id, CancellationToken cancellationToken)
    {
        Igreja igreja = await ObterEntidadeAsync(id, cancellationToken);

        igreja.Desativar();

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }



    private async Task<Igreja> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(id, cancellationToken);
         EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);
        if (igreja is null || !escopo.AlcancaIgreja(igreja.Id))
        {
            throw new NaoEncontradoException($"Igreja '{id}' não encontrada.");
        }

        return igreja;
    }

    private async Task GarantirCnpjDisponivelAsync(string cnpj, CancellationToken cancellationToken)
    {
        if (await igrejaRepository.ExisteComCnpjAsync(cnpj, cancellationToken))
        {
            throw new ConflitoException($"Já existe uma igreja com o CNPJ '{cnpj}'.");
        }
    }
}
