using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Aniversariantes;
using Temple.Application.Apresentacoes;  
using Temple.Application.Batismos;        
using Temple.Application.Certificados;
using Temple.Application.Comum.Acesso;
using Temple.Application.Igrejas;
using Temple.Application.Membros;
using Temple.Application.Autenticacao;
using Temple.Application.Usuarios;

namespace Temple.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("pt-BR");

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ProvedorDeEscopo>();
        services.AddScoped<IgrejaService>();
        services.AddScoped<MembroService>();
        services.AddScoped<AutenticacaoService>();
        services.AddScoped<UsuarioService>();
        services.AddScoped<AniversarianteService>();
        services.AddScoped<CertificadoService>();
        services.AddScoped<BatismoService>();        
        services.AddScoped<ApresentacaoService>();    
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
