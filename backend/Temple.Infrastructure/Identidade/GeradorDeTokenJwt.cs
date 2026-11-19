using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Temple.Application.Autenticacao;

namespace Temple.Infrastructure.Identidade;

internal sealed class GeradorDeTokenJwt(OpcoesJwt opcoes) : IGeradorDeToken
{
    private readonly JsonWebTokenHandler _manipulador = new();

    public LoginResponse Gerar(UsuarioAutenticado usuario)
    {
        DateTimeOffset agora = DateTimeOffset.UtcNow;
        DateTimeOffset expiraEm = agora.AddHours(opcoes.ExpiracaoEmHoras);

        ClaimsIdentity identidade = new(
        [
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ]);

        SecurityTokenDescriptor descritor = new()
        {
            Issuer = opcoes.Emissor,
            Audience = opcoes.Audiencia,
            Subject = identidade,
            IssuedAt = agora.UtcDateTime,
            NotBefore = agora.UtcDateTime,
            Expires = expiraEm.UtcDateTime,
            SigningCredentials = new SigningCredentials(opcoes.CriarChaveDeAssinatura(), SecurityAlgorithms.HmacSha256)
        };

        string token = _manipulador.CreateToken(descritor);

        return new LoginResponse(token, expiraEm);
    }
}
