using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Temple.Application.Autenticacao;

namespace Temple.Api.Controllers;

[ApiController]
[Route("api/autenticacao")]
public sealed class AutenticacaoController(AutenticacaoService autenticacaoService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        LoginResponse resposta = await autenticacaoService.LoginAsync(request, cancellationToken);

        return Ok(resposta);
    }

    [HttpGet("me")]
    [ProducesResponseType<UsuarioLogadoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioLogadoResponse>> ObterUsuarioLogado(CancellationToken cancellationToken)
    {
        Claim? claimDoUsuario = User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claimDoUsuario is null || !Guid.TryParse(claimDoUsuario.Value, out Guid usuarioId))
        {
            throw new InvalidOperationException("O token não contém o identificador do usuário.");
        }

        UsuarioLogadoResponse usuario = await autenticacaoService.ObterUsuarioLogadoAsync(usuarioId, cancellationToken);

        return Ok(usuario);
    }
}
