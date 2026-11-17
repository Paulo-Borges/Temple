using Microsoft.AspNetCore.Mvc;
using Temple.Application.Igrejas;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/igrejas")]
public sealed class IgrejasController(IgrejaService igrejaService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<IgrejaResponse>> CriarMatriz(CriarIgrejaRequest request, CancellationToken cancellationToken)
    {
        IgrejaResponse matriz = await igrejaService.CriarMatrizAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = matriz.Id }, matriz);
    }

    [HttpPost("{matrizId:guid}/filiais")]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<IgrejaResponse>> CriarFilial(Guid matrizId, CriarIgrejaRequest request, CancellationToken cancellationToken)
    {
        IgrejaResponse filial = await igrejaService.CriarFilialAsync(matrizId, request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = filial.Id }, filial);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IgrejaResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        IgrejaResponse igreja = await igrejaService.ObterPorIdAsync(id, cancellationToken);

        return Ok(igreja);
    }
}
