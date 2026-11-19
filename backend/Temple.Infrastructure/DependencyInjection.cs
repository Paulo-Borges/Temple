using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Comum.Persistencia;
using Temple.Application.Igrejas;
using Temple.Application.Membros;
using Temple.Infrastructure.Persistence;
using Temple.Infrastructure.Persistence.Repositorios;
using Temple.Infrastructure.Persistence.Seed;

namespace Temple.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        bool usarDadosDeDesenvolvimento)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<TempleDbContext>(options =>
        {
            options.UseSqlServer(connectionString);

            if (usarDadosDeDesenvolvimento)
            {
                options.UseSeeding(DadosDeDesenvolvimento.Semear);
                options.UseAsyncSeeding(DadosDeDesenvolvimento.SemearAsync);
            }
        });

        services.AddScoped<IIgrejaRepository, IgrejaRepository>();
        services.AddScoped<IMembroRepository, MembroRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
