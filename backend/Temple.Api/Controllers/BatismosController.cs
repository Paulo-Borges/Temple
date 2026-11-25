using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Batismos;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/membros/{membroId:guid}/batismo")]
[Authorize(Policy = Permissoes.GerenciarRegistros)]
public sealed class BatismosController(BatismoService batismoService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<BatismoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BatismoResponse>> Registrar(Guid membroId, BatismoRequest request, CancellationToken cancellationToken)
    {
        BatismoResponse batismo = await batismoService.RegistrarAsync(membroId, request, cancellationToken);

        return CreatedAtAction(nameof(Obter), new { membroId }, batismo);
    }

    [HttpGet]
    [ProducesResponseType<BatismoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatismoResponse>> Obter(Guid membroId, CancellationToken cancellationToken)
    {
        BatismoResponse batismo = await batismoService.ObterAsync(membroId, cancellationToken);

        return Ok(batismo);
    }

    [HttpPut]
    [ProducesResponseType<BatismoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<BatismoResponse>> Corrigir(Guid membroId, BatismoRequest request, CancellationToken cancellationToken)
    {
        BatismoResponse batismo = await batismoService.CorrigirAsync(membroId, request, cancellationToken);

        return Ok(batismo);
    }
}