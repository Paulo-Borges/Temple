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

        [HttpGet]
    [ProducesResponseType<IReadOnlyList<IgrejaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<IgrejaResponse>>> Listar([FromQuery] bool? ativa, CancellationToken cancellationToken)
    {
        IReadOnlyList<IgrejaResponse> igrejas = await igrejaService.ListarAsync(ativa, cancellationToken);

        return Ok(igrejas);
    }

    [HttpGet("{id:guid}/filiais")]
    [ProducesResponseType<IReadOnlyList<IgrejaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<IgrejaResponse>>> ListarFiliais(Guid id, CancellationToken cancellationToken)
    {
        IReadOnlyList<IgrejaResponse> filiais = await igrejaService.ListarFiliaisAsync(id, cancellationToken);

        return Ok(filiais);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IgrejaResponse>> Atualizar(Guid id, AtualizarIgrejaRequest request, CancellationToken cancellationToken)
    {
        IgrejaResponse igreja = await igrejaService.AtualizarAsync(id, request, cancellationToken);

        return Ok(igreja);
    }

    [HttpPost("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Ativar(Guid id, CancellationToken cancellationToken)
    {
        await igrejaService.AtivarAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Desativar(Guid id, CancellationToken cancellationToken)
    {
        await igrejaService.DesativarAsync(id, cancellationToken);

        return NoContent();
    }

}
