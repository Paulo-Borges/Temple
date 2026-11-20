using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Temple.Domain.Enums;

namespace Temple.Infrastructure.Identidade;

public static class SemeadorDoPastorInicial
{
    public static async Task CriarPastorInicialAsync(this IServiceProvider servicos, string nome, string email, string senha)
    {
        await using AsyncServiceScope escopo = servicos.CreateAsyncScope();
        UserManager<Usuario> userManager = escopo.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        Usuario? pastor = await userManager.FindByEmailAsync(email);

        if (pastor is null)
        {
            pastor = new Usuario(nome, email);
            GarantirSucesso(await userManager.CreateAsync(pastor, senha));
        }

        string papelDoPastor = ConversorDePerfis.ParaNomeDoPapel(Perfil.Pastor);

        if (!await userManager.IsInRoleAsync(pastor, papelDoPastor))
        {
            GarantirSucesso(await userManager.AddToRoleAsync(pastor, papelDoPastor));
        }
    }

    private static void GarantirSucesso(IdentityResult resultado)
    {
        if (!resultado.Succeeded)
        {
            string erros = string.Join(" ", resultado.Errors.Select(erro => erro.Description));
            throw new InvalidOperationException($"Não foi possível preparar o Pastor inicial. {erros}");
        }
    }
}

