using FluentValidation;
using Temple.Application.Autenticacao;
using Temple.Application.Comum;
using Temple.Application.Comum.Excecoes;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

public sealed class MembroService(
    IMembroRepository membroRepository,
    IIgrejaRepository igrejaRepository,
    IUnitOfWork unitOfWork,
    IServicoDeIdentidade servicoDeIdentidade,
    IValidator<CriarMembroRequest> criarMembroValidator,
    IValidator<AtualizarMembroRequest> atualizarMembroValidator,
    IValidator<PromoverCargoRequest> promoverCargoValidator,
    IValidator<ListarMembrosRequest> listarMembrosValidator)
{
    public async Task<MembroResponse> CriarAsync(CriarMembroRequest request, CancellationToken cancellationToken)
    {
        await criarMembroValidator.ValidateAndThrowAsync(request, cancellationToken);

        Igreja igreja = await ObterIgrejaAsync(request.IgrejaId, cancellationToken);
        Membro membro = request.ParaMembro(igreja);

        membroRepository.Adicionar(membro);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return membro.ParaResponse();
    }

    public async Task<MembroResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro membro = await ObterEntidadeAsync(id, cancellationToken);

        return membro.ParaResponse();
    }

        public async Task<MembroResponse> ObterMeuCadastroAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        UsuarioAutenticado? usuario = await servicoDeIdentidade.ObterPorIdAsync(usuarioId, cancellationToken);

        if (usuario is null)
        {
            throw new NaoAutorizadoException("O usuário do token não existe mais.");
        }

        if (usuario.MembroId is not Guid membroId)
        {
            throw new NaoEncontradoException("O seu usuário não está vinculado a um cadastro de membro.");
        }

        Membro membro = await ObterEntidadeAsync(membroId, cancellationToken);

        return membro.ParaResponse();
    }


    public async Task<PaginaResponse<MembroResponse>> ListarAsync(ListarMembrosRequest filtro, CancellationToken cancellationToken)
    {
        await listarMembrosValidator.ValidateAndThrowAsync(filtro, cancellationToken);

        ResultadoPaginado<Membro> resultado = await membroRepository.ListarAsync(filtro, cancellationToken);

        List<MembroResponse> itens = resultado.Itens
            .Select(membro => membro.ParaResponse())
            .ToList();

        int totalPaginas = (resultado.TotalItens + filtro.TamanhoPagina - 1) / filtro.TamanhoPagina;

        return new PaginaResponse<MembroResponse>(itens, filtro.Pagina, filtro.TamanhoPagina, resultado.TotalItens, totalPaginas);
    }

    public async Task<MembroResponse> AtualizarAsync(Guid id, AtualizarMembroRequest request, CancellationToken cancellationToken)
    {
        await atualizarMembroValidator.ValidateAndThrowAsync(request, cancellationToken);

        Membro membro = await ObterEntidadeAsync(id, cancellationToken);

        membro.AlterarNome(request.Nome);
        membro.AlterarEndereco(request.Endereco.ParaEndereco());
        membro.AlterarGrupo(request.Grupo);

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return membro.ParaResponse();
    }

    public async Task AtivarAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro membro = await ObterEntidadeAsync(id, cancellationToken);
        Igreja igreja = await ObterIgrejaAsync(membro.IgrejaId, cancellationToken);

        membro.Ativar(igreja);

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task DesativarAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro membro = await ObterEntidadeAsync(id, cancellationToken);

        membro.Desativar();

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task<MembroResponse> PromoverCargoAsync(Guid id, PromoverCargoRequest request, CancellationToken cancellationToken)
    {
        await promoverCargoValidator.ValidateAndThrowAsync(request, cancellationToken);

        Membro membro = await ObterEntidadeAsync(id, cancellationToken);

        membro.PromoverCargo(request.NovoCargo, request.DataPromocao);

        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);

        return membro.ParaResponse();
    }

    public async Task<IReadOnlyList<HistoricoCargoResponse>> ListarHistoricoDeCargosAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro membro = await ObterEntidadeAsync(id, cancellationToken);

        return membro.HistoricoCargos
            .OrderBy(historico => historico.DataPromocao)
            .Select(historico => historico.ParaResponse())
            .ToList();
    }

    private async Task<Membro> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        Membro? membro = await membroRepository.ObterPorIdAsync(id, cancellationToken);

        if (membro is null)
        {
            throw new NaoEncontradoException($"Membro '{id}' não encontrado.");
        }

        return membro;
    }

    private async Task<Igreja> ObterIgrejaAsync(Guid igrejaId, CancellationToken cancellationToken)
    {
        Igreja? igreja = await igrejaRepository.ObterPorIdAsync(igrejaId, cancellationToken);

        if (igreja is null)
        {
            throw new NaoEncontradoException($"Igreja '{igrejaId}' não encontrada.");
        }

        return igreja;
    }
}
