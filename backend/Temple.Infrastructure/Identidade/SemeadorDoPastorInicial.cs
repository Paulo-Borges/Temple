using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Temple.Infrastructure.Identidade;

public static class SemeadorDoPastorInicial
{
    public static async Task CriarPastorInicialAsync(this IServiceProvider servicos, string nome, string email, string senha)
    {
        await using AsyncServiceScope escopo = servicos.CreateAsyncScope();
        UserManager<Usuario> userManager = escopo.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        Usuario? existente = await userManager.FindByEmailAsync(email);

        if (existente is not null)
        {
            return;
        }

        Usuario pastor = new(nome, email);
        IdentityResult resultado = await userManager.CreateAsync(pastor, senha);

        if (!resultado.Succeeded)
        {
            string erros = string.Join(" ", resultado.Errors.Select(erro => erro.Description));
            throw new InvalidOperationException($"Não foi possível criar o Pastor inicial. {erros}");
        }
    }
}
