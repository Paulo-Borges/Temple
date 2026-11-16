using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class HistoricoCargoConfiguration : IEntityTypeConfiguration<HistoricoCargo>
{
    public void Configure(EntityTypeBuilder<HistoricoCargo> builder)
    {
        builder.ToTable("HistoricoCargos");

        builder.HasKey(historico => historico.Id);

        builder.Property(historico => historico.Id)
            .ValueGeneratedNever();

        builder.Property(historico => historico.CargoAnterior)
            .IsRequired();

        builder.Property(historico => historico.CargoNovo)
            .IsRequired();

        builder.Property(historico => historico.DataPromocao)
            .IsRequired();
    }
}
