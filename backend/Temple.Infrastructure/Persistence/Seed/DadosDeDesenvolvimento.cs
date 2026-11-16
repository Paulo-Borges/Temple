using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities;
using Temple.Domain.ValueObjects;

namespace Temple.Infrastructure.Persistence.Seed;

public static class DadosDeDesenvolvimento
{
    public static void Semear(DbContext context, bool _)
    {
        DbSet<Igreja> igrejas = context.Set<Igreja>();

        if (igrejas.Any(igreja => igreja.IgrejaMatrizId == null))
        {
            return;
        }

        igrejas.Add(CriarMatrizDeExemplo());
        context.SaveChanges();
    }

    public static async Task SemearAsync(DbContext context, bool _, CancellationToken cancellationToken)
    {
        DbSet<Igreja> igrejas = context.Set<Igreja>();

        if (await igrejas.AnyAsync(igreja => igreja.IgrejaMatrizId == null, cancellationToken))
        {
            return;
        }

        igrejas.Add(CriarMatrizDeExemplo());
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Igreja CriarMatrizDeExemplo()
    {
        Endereco endereco = new(
            logradouro: "Rua das Oliveiras",
            numero: "100",
            complemento: null,
            bairro: "Centro",
            cidade: "São Paulo",
            uf: "SP",
            cep: "01001000");

        return Igreja.CriarMatriz(
            nome: "Igreja Sede Exemplo",
            cnpj: "00000000000191",
            telefone: "1130000000",
            dataFundacao: new DateOnly(1990, 1, 1),
            ativa: true,
            endereco: endereco);
    }
}
