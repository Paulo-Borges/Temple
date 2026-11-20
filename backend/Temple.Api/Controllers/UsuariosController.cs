using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temple.Api.Autorizacao;
using Temple.Application.Usuarios;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Policy = Permissoes.GerenciarUsuarios)]
public sealed class UsuariosController(UsuarioService usuarioService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Criar(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        UsuarioResponse usuario = await usuarioService.CriarAsync(request, cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UsuarioResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> Listar(CancellationToken cancellationToken)
    {
        IReadOnlyList<UsuarioResponse> usuarios = await usuarioService.ListarAsync(cancellationToken);

        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        UsuarioResponse usuario = await usuarioService.ObterPorIdAsync(id, cancellationToken);

        return Ok(usuario);
    }

    [HttpPut("{id:guid}/perfis")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UsuarioResponse>> AlterarPerfis(Guid id, AlterarPerfisRequest request, CancellationToken cancellationToken)
    {
        UsuarioResponse usuario = await usuarioService.AlterarPerfisAsync(id, request, cancellationToken);

        return Ok(usuario);
    }
}
