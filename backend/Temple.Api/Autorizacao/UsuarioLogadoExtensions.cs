using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Temple.Api.Autorizacao;

internal static class UsuarioLogadoExtensions
{
    public static Guid ObterIdDoUsuario(this ClaimsPrincipal usuario)
    {
        Claim? claimDoUsuario = usuario.FindFirst(JwtRegisteredClaimNames.Sub);

        if (claimDoUsuario is null || !Guid.TryParse(claimDoUsuario.Value, out Guid usuarioId))
        {
            throw new InvalidOperationException("O token não contém o identificador do usuário.");
        }

        return usuarioId;
    }
}
