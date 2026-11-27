using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure; 
using Temple.Application.Apresentacoes;
using Temple.Application.Autenticacao;
using Temple.Application.Batismos; 
using Temple.Application.Certificados;  
using Temple.Application.Comum.Persistencia;
using Temple.Application.Fechamentos;
using Temple.Application.Igrejas;
using Temple.Application.Lancamentos;
using Temple.Application.Membros;
using Temple.Application.RelatoriosFinanceiros;
using Temple.Infrastructure.Certificados;
using Temple.Infrastructure.Identidade;
using Temple.Infrastructure.Persistence;
using Temple.Infrastructure.Persistence.Repositorios;
using Temple.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Temple.Application.Usuarios;



namespace Temple.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        bool usarDadosDeDesenvolvimento,
        OpcoesJwt opcoesJwt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentNullException.ThrowIfNull(opcoesJwt);

        services.AddDbContext<TempleDbContext>(options =>
        {
            options.UseSqlServer(connectionString);

            if (usarDadosDeDesenvolvimento)
            {
                options.UseSeeding(DadosDeDesenvolvimento.Semear);
                options.UseAsyncSeeding(DadosDeDesenvolvimento.SemearAsync);
            }
        });

        services.AddIdentityCore<Usuario>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddErrorDescriber<DescritorDeErrosEmPortugues>()
            .AddEntityFrameworkStores<TempleDbContext>();

        services.AddSingleton(opcoesJwt);
        services.AddSingleton<IGeradorDeToken, GeradorDeTokenJwt>();
        services.AddScoped<IServicoDeIdentidade, ServicoDeIdentidade>();
        services.AddScoped<IGerenciadorDeUsuarios, GerenciadorDeUsuarios>();

        services.AddScoped<IIgrejaRepository, IgrejaRepository>();
        services.AddScoped<IMembroRepository, MembroRepository>();
        services.AddScoped<ICertificadoRepository, CertificadoRepository>();
        services.AddScoped<IBatismoRepository, BatismoRepository>();            
        services.AddScoped<IApresentacaoRepository, ApresentacaoRepository>();  
        services.AddScoped<ILancamentoRepository, LancamentoRepository>(); 
        services.AddScoped<IRelatorioFinanceiroRepository, RelatorioFinanceiroRepository>();
        services.AddScoped<IFechamentoRepository, FechamentoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        QuestPDF.Settings.License = LicenseType.Community;                    
        services.AddSingleton<IGeradorDeCertificadoPdf, GeradorDeCertificadoPdf>();

        return services;
    }
}
