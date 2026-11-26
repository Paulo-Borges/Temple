using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.RelatoriosFinanceiros;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/relatorios-financeiros")]
[Authorize(Policy = Permissoes.LancarFinanceiro)]
public sealed class RelatoriosFinanceirosController(RelatorioFinanceiroService relatorioService) : ControllerBase
{
    [HttpGet("mensal")]
    [ProducesResponseType<RelatorioMensalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatorioMensalResponse>> ObterMensal([FromQuery] RelatorioFinanceiroRequest request, CancellationToken cancellationToken)
    {
        RelatorioMensalResponse relatorio = await relatorioService.ObterMensalAsync(request, cancellationToken);

        return Ok(relatorio);
    }

    [HttpGet("consolidado")]
    [ProducesResponseType<RelatorioConsolidadoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RelatorioConsolidadoResponse>> ObterConsolidado([FromQuery] RelatorioFinanceiroRequest request, CancellationToken cancellationToken)
    {
        RelatorioConsolidadoResponse relatorio = await relatorioService.ObterConsolidadoAsync(request, cancellationToken);

        return Ok(relatorio);
    }
}