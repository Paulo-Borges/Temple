using FluentValidation;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Domain.Entities;

namespace Temple.Application.Igrejas;

public sealed class IgrejaService(
    IIgrejaRepository igrejaRepository,
    IUnitOfWork unitOfWork,
    IValidator<CriarIgrejaRequest> criarIgrejaValidator)
{
    public async Task<IgrejaResponse> CriarMatrizAsync(CriarIgrejaRequest request, CancellationToken cancellationToken)
    {
        await criarIgrejaValidator.ValidateAndThrowAsync(request, cancellationToken);

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

    private async Task<Igreja> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(id, cancellationToken);

        if (igreja is null)
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
