using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Comum;
using Temple.Application.Membros;


namespace Temple.Api.Controllers;

[ApiController]
[Route("api/membros")]
public sealed class MembrosController(MembroService membroService) : ControllerBase
{
    [Authorize(Policy = Permissoes.GerenciarMembros)]
    [HttpPost]
    [ProducesResponseType<MembroResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MembroResponse>> Criar(CriarMembroRequest request, CancellationToken cancellationToken)
    {
        MembroResponse membro = await membroService.CriarAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = membro.Id }, membro);
    }

    [Authorize(Policy = Permissoes.VerMembros)]
    [HttpGet]
    [ProducesResponseType<PaginaResponse<MembroResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginaResponse<MembroResponse>>> Listar([FromQuery] ListarMembrosRequest filtro, CancellationToken cancellationToken)
    {
        PaginaResponse<MembroResponse> pagina = await membroService.ListarAsync(filtro, cancellationToken);

        return Ok(pagina);
    }

    [Authorize(Policy = Permissoes.VerMembros)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<MembroResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembroResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        MembroResponse membro = await membroService.ObterPorIdAsync(id, cancellationToken);

        return Ok(membro);
    }

    [Authorize(Policy = Permissoes.GerenciarMembros)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType<MembroResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembroResponse>> Atualizar(Guid id, AtualizarMembroRequest request, CancellationToken cancellationToken)
    {
        MembroResponse membro = await membroService.AtualizarAsync(id, request, cancellationToken);

        return Ok(membro);
    }

    [Authorize(Policy = Permissoes.GerenciarMembros)]
    [HttpPost("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Ativar(Guid id, CancellationToken cancellationToken)
    {
        await membroService.AtivarAsync(id, cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = Permissoes.GerenciarMembros)]
    [HttpPost("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Desativar(Guid id, CancellationToken cancellationToken)
    {
        await membroService.DesativarAsync(id, cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = Permissoes.PromoverMembros)]
    [HttpPost("{id:guid}/promocoes")]
    [ProducesResponseType<MembroResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MembroResponse>> PromoverCargo(Guid id, PromoverCargoRequest request, CancellationToken cancellationToken)
    {
        MembroResponse membro = await membroService.PromoverCargoAsync(id, request, cancellationToken);

        return Ok(membro);
    }

    [Authorize(Policy = Permissoes.VerMembros)]
    [HttpGet("{id:guid}/historico-cargos")]
    [ProducesResponseType<IReadOnlyList<HistoricoCargoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<HistoricoCargoResponse>>> ListarHistoricoDeCargos(Guid id, CancellationToken cancellationToken)
    {
        IReadOnlyList<HistoricoCargoResponse> historico = await membroService.ListarHistoricoDeCargosAsync(id, cancellationToken);

        return Ok(historico);
    }
}
