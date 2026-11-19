namespace Temple.Api.Configuracao;

internal static class ConfiguracaoExtensions
{
    public static string ObterObrigatorio(this IConfiguration configuracao, string chave)
    {
        string? valor = configuracao[chave];

        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new InvalidOperationException($"A configuração '{chave}' não foi definida.");
        }

        return valor;
    }
}
