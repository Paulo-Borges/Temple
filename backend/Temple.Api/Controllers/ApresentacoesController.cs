using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Apresentacoes;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/apresentacoes")]
[Authorize(Policy = Permissoes.GerenciarRegistros)]
public sealed class ApresentacoesController(ApresentacaoService apresentacaoService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ApresentacaoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApresentacaoResponse>> Criar(CriarApresentacaoRequest request, CancellationToken cancellationToken)
    {
        ApresentacaoResponse apresentacao = await apresentacaoService.CriarAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = apresentacao.Id }, apresentacao);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ApresentacaoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ApresentacaoResponse>>> Listar([FromQuery] Guid? igrejaId, CancellationToken cancellationToken)
    {
        IReadOnlyList<ApresentacaoResponse> apresentacoes = await apresentacaoService.ListarAsync(igrejaId, cancellationToken);

        return Ok(apresentacoes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ApresentacaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApresentacaoResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        ApresentacaoResponse apresentacao = await apresentacaoService.ObterPorIdAsync(id, cancellationToken);

        return Ok(apresentacao);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ApresentacaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApresentacaoResponse>> Atualizar(Guid id, AtualizarApresentacaoRequest request, CancellationToken cancellationToken)
    {
        ApresentacaoResponse apresentacao = await apresentacaoService.AtualizarAsync(id, request, cancellationToken);

        return Ok(apresentacao);
    }
}