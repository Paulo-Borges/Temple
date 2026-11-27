using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Fechamentos;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/fechamentos")]
[Authorize(Policy = Permissoes.LancarFinanceiro)]
public sealed class FechamentosController(FechamentoService fechamentoService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<FechamentoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<FechamentoResponse>>> Listar([FromQuery] Guid igrejaId, CancellationToken cancellationToken)
    {
        IReadOnlyList<FechamentoResponse> fechamentos = await fechamentoService.ListarAsync(igrejaId, cancellationToken);

        return Ok(fechamentos);
    }

    [HttpPost("fechar")]
    [Authorize(Policy = Permissoes.FecharMesFinanceiro)]
    [ProducesResponseType<FechamentoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<FechamentoResponse>> Fechar(FecharMesRequest request, CancellationToken cancellationToken)
    {
        FechamentoResponse fechamento = await fechamentoService.FecharAsync(request, cancellationToken);

        return Ok(fechamento);
    }

    [HttpPost("reabrir")]
    [Authorize(Policy = Permissoes.ReabrirMesFinanceiro)]
    [ProducesResponseType<FechamentoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<FechamentoResponse>> Reabrir(ReabrirMesRequest request, CancellationToken cancellationToken)
    {
        FechamentoResponse fechamento = await fechamentoService.ReabrirAsync(request, cancellationToken);

        return Ok(fechamento);
    }
}