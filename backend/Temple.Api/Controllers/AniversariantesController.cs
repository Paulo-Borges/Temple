using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Aniversariantes;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/membros/aniversariantes")]
public sealed class AniversariantesController(AniversarianteService aniversarianteService) : ControllerBase
{
    [Authorize(Policy = Permissoes.VerAniversariantes)]
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AniversarianteResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<AniversarianteResponse>>> Listar([FromQuery] ListarAniversariantesRequest filtro, CancellationToken cancellationToken)
    {
        IReadOnlyList<AniversarianteResponse> aniversariantes = await aniversarianteService.ListarAsync(filtro, cancellationToken);

        return Ok(aniversariantes);
    }
}