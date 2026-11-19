using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Temple.Infrastructure.Identidade;

public sealed class OpcoesJwt
{
    public const int TamanhoMinimoDaChaveEmBytes = 32;

    private readonly byte[] _chave;

    public OpcoesJwt(string emissor, string audiencia, string chave, int expiracaoEmHoras)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emissor);
        ArgumentException.ThrowIfNullOrWhiteSpace(audiencia);
        ArgumentException.ThrowIfNullOrWhiteSpace(chave);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(expiracaoEmHoras);

        byte[] bytesDaChave = Encoding.UTF8.GetBytes(chave);

        if (bytesDaChave.Length < TamanhoMinimoDaChaveEmBytes)
        {
            throw new ArgumentException(
                $"A chave do JWT deve ter pelo menos {TamanhoMinimoDaChaveEmBytes} bytes (256 bits).",
                nameof(chave));
        }

        Emissor = emissor;
        Audiencia = audiencia;
        ExpiracaoEmHoras = expiracaoEmHoras;
        _chave = bytesDaChave;
    }

    public string Emissor { get; }
    public string Audiencia { get; }
    public int ExpiracaoEmHoras { get; }

    public SymmetricSecurityKey CriarChaveDeAssinatura()
    {
        return new SymmetricSecurityKey(_chave);
    }
}
