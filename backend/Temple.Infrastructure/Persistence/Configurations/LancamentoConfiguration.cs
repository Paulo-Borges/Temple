using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;
using Temple.Domain.Enums;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class LancamentoConfiguration : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> builder)
    {
        // Os números dos enums vêm do próprio enum, sem número solto no SQL.
        int dizimo = (int)TipoLancamento.Dizimo;
        int oferta = (int)TipoLancamento.Oferta;
        int outraEntrada = (int)TipoLancamento.OutraEntrada;
        int despesa = (int)TipoLancamento.Despesa;

        builder.ToTable("Lancamentos", tabela =>
        {
            tabela.HasCheckConstraint(
                "CK_Lancamentos_Valor",
                "[Valor] > 0");

            // Dízimo tem membro, oferta pode ter ou não, outra entrada e despesa nunca têm.
            tabela.HasCheckConstraint(
                "CK_Lancamentos_Membro",
                $"([Tipo] = {dizimo} AND [MembroId] IS NOT NULL) OR [Tipo] = {oferta} OR ([Tipo] IN ({outraEntrada}, {despesa}) AND [MembroId] IS NULL)");

            // Só a despesa tem categoria, e toda despesa tem.
            tabela.HasCheckConstraint(
                "CK_Lancamentos_Categoria",
                $"([Tipo] = {despesa} AND [Categoria] IS NOT NULL) OR ([Tipo] <> {despesa} AND [Categoria] IS NULL)");

            // Os quatro campos do estorno ficam todos nulos ou todos preenchidos.
            tabela.HasCheckConstraint(
                "CK_Lancamentos_Estorno",
                "([MotivoDoEstorno] IS NULL AND [EstornadoPorId] IS NULL AND [EstornadoPorNome] IS NULL AND [EstornadoEm] IS NULL)"
                + " OR ([MotivoDoEstorno] IS NOT NULL AND [EstornadoPorId] IS NOT NULL AND [EstornadoPorNome] IS NOT NULL AND [EstornadoEm] IS NOT NULL)");
        });

        builder.HasKey(lancamento => lancamento.Id);

        builder.Property(lancamento => lancamento.Id)
            .ValueGeneratedNever();

        builder.Property(lancamento => lancamento.Tipo)
            .IsRequired();

        builder.Property(lancamento => lancamento.Data)
            .IsRequired();

        builder.Property(lancamento => lancamento.Valor)
            .HasPrecision(18, Lancamento.CasasDecimais)
            .IsRequired();

        builder.Property(lancamento => lancamento.FormaPagamento)
            .IsRequired();

        builder.Property(lancamento => lancamento.Descricao)
            .HasMaxLength(Lancamento.TamanhoMaximoDescricao);

        builder.Property(lancamento => lancamento.RegistradoPorNome)
            .HasMaxLength(Lancamento.TamanhoMaximoNomeDoAutor)
            .IsRequired();

        builder.Property(lancamento => lancamento.RegistradoEm)
            .IsRequired();

        builder.Property(lancamento => lancamento.MotivoDoEstorno)
            .HasMaxLength(Lancamento.TamanhoMaximoMotivoDoEstorno);

        builder.Property(lancamento => lancamento.EstornadoPorNome)
            .HasMaxLength(Lancamento.TamanhoMaximoNomeDoAutor);

        builder.HasOne<Igreja>()
            .WithMany()
            .HasForeignKey(lancamento => lancamento.IgrejaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Membro>()
            .WithMany()
            .HasForeignKey(lancamento => lancamento.MembroId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relatórios e fechamento do mês: lançamentos de uma igreja num período.
        builder.HasIndex(lancamento => new { lancamento.IgrejaId, lancamento.Data });

        // Contribuições de um membro num período.
        builder.HasIndex(lancamento => new { lancamento.MembroId, lancamento.Data });
    }
}