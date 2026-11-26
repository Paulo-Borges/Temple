using Microsoft.AspNetCore.Authorization;
using Temple.Application.Comum;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Lancamentos;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/lancamentos")]
[Authorize(Policy = Permissoes.LancarFinanceiro)]
public sealed class LancamentosController(LancamentoService lancamentoService) : ControllerBase
{
    [HttpPost("dizimos")]
    [ProducesResponseType<LancamentoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LancamentoResponse>> RegistrarDizimo(CriarDizimoRequest request, CancellationToken cancellationToken)
    {
        LancamentoResponse lancamento = await lancamentoService.RegistrarDizimoAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = lancamento.Id }, lancamento);
    }

    [HttpPost("ofertas")]
    [ProducesResponseType<LancamentoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LancamentoResponse>> RegistrarOferta(CriarOfertaRequest request, CancellationToken cancellationToken)
    {
        LancamentoResponse lancamento = await lancamentoService.RegistrarOfertaAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = lancamento.Id }, lancamento);
    }

    [HttpPost("outras-entradas")]
    [ProducesResponseType<LancamentoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LancamentoResponse>> RegistrarOutraEntrada(CriarOutraEntradaRequest request, CancellationToken cancellationToken)
    {
        LancamentoResponse lancamento = await lancamentoService.RegistrarOutraEntradaAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = lancamento.Id }, lancamento);
    }

    [HttpPost("despesas")]
    [ProducesResponseType<LancamentoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LancamentoResponse>> RegistrarDespesa(CriarDespesaRequest request, CancellationToken cancellationToken)
    {
        LancamentoResponse lancamento = await lancamentoService.RegistrarDespesaAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = lancamento.Id }, lancamento);
    }

    [HttpGet]
    [ProducesResponseType<PaginaResponse<LancamentoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginaResponse<LancamentoResponse>>> Listar([FromQuery] ListarLancamentosRequest filtro, CancellationToken cancellationToken)
    {
        PaginaResponse<LancamentoResponse> pagina = await lancamentoService.ListarAsync(filtro, cancellationToken);

        return Ok(pagina);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<LancamentoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LancamentoResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        LancamentoResponse lancamento = await lancamentoService.ObterPorIdAsync(id, cancellationToken);

        return Ok(lancamento);
    }

    // Exige as duas políticas: a do controller (lançar) e esta (estornar).
    [HttpPost("{id:guid}/estornar")]
    [Authorize(Policy = Permissoes.EstornarFinanceiro)]
    [ProducesResponseType<LancamentoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LancamentoResponse>> Estornar(Guid id, EstornarLancamentoRequest request, CancellationToken cancellationToken)
    {
        LancamentoResponse lancamento = await lancamentoService.EstornarAsync(id, request, cancellationToken);

        return Ok(lancamento);
    }
}