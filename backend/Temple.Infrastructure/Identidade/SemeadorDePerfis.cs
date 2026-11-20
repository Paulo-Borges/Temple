using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Temple.Domain.Enums;

namespace Temple.Infrastructure.Identidade;

public static class SemeadorDePerfis
{
    public static async Task CriarPerfisAsync(this IServiceProvider servicos)
    {
        await using AsyncServiceScope escopo = servicos.CreateAsyncScope();
        RoleManager<IdentityRole<Guid>> roleManager = escopo.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (Perfil perfil in Enum.GetValues<Perfil>())
        {
            string nomeDoPapel = ConversorDePerfis.ParaNomeDoPapel(perfil);

            if (await roleManager.RoleExistsAsync(nomeDoPapel))
            {
                continue;
            }

            IdentityRole<Guid> papel = new(nomeDoPapel)
            {
                Id = Guid.NewGuid()
            };

            IdentityResult resultado = await roleManager.CreateAsync(papel);

            if (!resultado.Succeeded)
            {
                string erros = string.Join(" ", resultado.Errors.Select(erro => erro.Description));
                throw new InvalidOperationException($"Não foi possível criar o perfil '{nomeDoPapel}'. {erros}");
            }
        }
    }
}
