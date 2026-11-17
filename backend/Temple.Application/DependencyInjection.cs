using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Igrejas;

namespace Temple.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("pt-BR");

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IgrejaService>();

        return services;
    }
}
