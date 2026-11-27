using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class FechamentoMensalConfiguration : IEntityTypeConfiguration<FechamentoMensal>
{
    public void Configure(EntityTypeBuilder<FechamentoMensal> builder)
    {
        builder.ToTable("FechamentosMensais", tabela =>
        {
            tabela.HasCheckConstraint(
                "CK_FechamentosMensais_Mes",
                "[Mes] BETWEEN 1 AND 12");

            // Os quatro campos da reabertura ficam todos nulos ou todos preenchidos.
            tabela.HasCheckConstraint(
                "CK_FechamentosMensais_Reabertura",
                "([MotivoDaReabertura] IS NULL AND [ReabertoPorId] IS NULL AND [ReabertoPorNome] IS NULL AND [ReabertoEm] IS NULL)"
                + " OR ([MotivoDaReabertura] IS NOT NULL AND [ReabertoPorId] IS NOT NULL AND [ReabertoPorNome] IS NOT NULL AND [ReabertoEm] IS NOT NULL)");
        });

        builder.HasKey(fechamento => fechamento.Id);

        builder.Property(fechamento => fechamento.Id)
            .ValueGeneratedNever();

        builder.Property(fechamento => fechamento.Ano)
            .IsRequired();

        builder.Property(fechamento => fechamento.Mes)
            .IsRequired();

        builder.Property(fechamento => fechamento.SaldoAnterior)
            .HasPrecision(18, Lancamento.CasasDecimais)
            .IsRequired();

        builder.Property(fechamento => fechamento.TotalEntradas)
            .HasPrecision(18, Lancamento.CasasDecimais)
            .IsRequired();

        builder.Property(fechamento => fechamento.TotalSaidas)
            .HasPrecision(18, Lancamento.CasasDecimais)
            .IsRequired();

        builder.Property(fechamento => fechamento.SaldoFinal)
            .HasPrecision(18, Lancamento.CasasDecimais)
            .IsRequired();

        builder.Property(fechamento => fechamento.FechadoPorNome)
            .HasMaxLength(FechamentoMensal.TamanhoMaximoNomeDoAutor)
            .IsRequired();

        builder.Property(fechamento => fechamento.FechadoEm)
            .IsRequired();

        builder.Property(fechamento => fechamento.MotivoDaReabertura)
            .HasMaxLength(FechamentoMensal.TamanhoMaximoMotivoDaReabertura);

        builder.Property(fechamento => fechamento.ReabertoPorNome)
            .HasMaxLength(FechamentoMensal.TamanhoMaximoNomeDoAutor);

        builder.HasOne<Igreja>()
            .WithMany()
            .HasForeignKey(fechamento => fechamento.IgrejaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cada mês tem no máximo um fechamento ativo. Os reabertos ficam no histórico.
        builder.HasIndex(fechamento => new { fechamento.IgrejaId, fechamento.Ano, fechamento.Mes })
            .IsUnique()
            .HasFilter("[ReabertoEm] IS NULL");
    }
}