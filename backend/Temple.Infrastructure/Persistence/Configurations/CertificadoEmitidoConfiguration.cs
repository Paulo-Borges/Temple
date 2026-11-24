using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class CertificadoEmitidoConfiguration : IEntityTypeConfiguration<CertificadoEmitido>
{
    public void Configure(EntityTypeBuilder<CertificadoEmitido> builder)
    {
        builder.ToTable("CertificadosEmitidos");

        builder.HasKey(certificado => certificado.Id);

        builder.Property(certificado => certificado.Id)
            .ValueGeneratedNever();

        builder.Property(certificado => certificado.Numero)
            .UseIdentityColumn();

        builder.HasIndex(certificado => certificado.Numero)
            .IsUnique();

        builder.Property(certificado => certificado.Tipo)
            .IsRequired();

        builder.Property(certificado => certificado.EmitidoPorNome)
            .HasMaxLength(CertificadoEmitido.TamanhoMaximoNomeDoEmissor)
            .IsRequired();

        builder.Property(certificado => certificado.EmitidoEm)
            .IsRequired();

        builder.HasOne<Membro>()
            .WithMany()
            .HasForeignKey(certificado => certificado.MembroId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HistoricoCargo>()
            .WithMany()
            .HasForeignKey(certificado => certificado.HistoricoCargoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Uma promoção tem um único certificado.
        builder.HasIndex(certificado => certificado.HistoricoCargoId)
            .IsUnique()
            .HasFilter("[HistoricoCargoId] IS NOT NULL");
    }
}