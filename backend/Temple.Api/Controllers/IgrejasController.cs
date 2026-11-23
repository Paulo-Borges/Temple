using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Igrejas;


namespace Temple.Api.Controllers;

[ApiController]
[Route("api/igrejas")]
public sealed class IgrejasController(IgrejaService igrejaService) : ControllerBase
{
    [Authorize(Policy = Permissoes.GerenciarIgrejas)]
    [HttpPost]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<IgrejaResponse>> CriarMatriz(CriarIgrejaRequest request, CancellationToken cancellationToken)
    {
        IgrejaResponse matriz = await igrejaService.CriarMatrizAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = matriz.Id }, matriz);
    }

    [Authorize(Policy = Permissoes.GerenciarIgrejas)]
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

    [Authorize(Policy = Permissoes.VerIgrejas)]
    [HttpGet("{id:guid}")]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IgrejaResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        IgrejaResponse igreja = await igrejaService.ObterPorIdAsync(id, cancellationToken);

        return Ok(igreja);
    }

    [Authorize(Policy = Permissoes.VerIgrejas)]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<IgrejaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<IgrejaResponse>>> Listar([FromQuery] bool? ativa, CancellationToken cancellationToken)
    {
        IReadOnlyList<IgrejaResponse> igrejas = await igrejaService.ListarAsync(ativa, cancellationToken);

        return Ok(igrejas);
    }

    [Authorize(Policy = Permissoes.VerIgrejas)]
    [HttpGet("{id:guid}/filiais")]
    [ProducesResponseType<IReadOnlyList<IgrejaResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<IgrejaResponse>>> ListarFiliais(Guid id, CancellationToken cancellationToken)
    {
        IReadOnlyList<IgrejaResponse> filiais = await igrejaService.ListarFiliaisAsync(id, cancellationToken);

        return Ok(filiais);
    }

    [Authorize(Policy = Permissoes.GerenciarIgrejas)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType<IgrejaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IgrejaResponse>> Atualizar(Guid id, AtualizarIgrejaRequest request, CancellationToken cancellationToken)
    {
        IgrejaResponse igreja = await igrejaService.AtualizarAsync(id, request, cancellationToken);

        return Ok(igreja);
    }

    [Authorize(Policy = Permissoes.GerenciarIgrejas)]
    [HttpPost("{id:guid}/ativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Ativar(Guid id, CancellationToken cancellationToken)
    {
        await igrejaService.AtivarAsync(id, cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = Permissoes.GerenciarIgrejas)]
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
