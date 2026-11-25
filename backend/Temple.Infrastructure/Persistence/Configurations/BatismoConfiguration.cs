using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class BatismoConfiguration : IEntityTypeConfiguration<Batismo>
{
    public void Configure(EntityTypeBuilder<Batismo> builder)
    {
        builder.ToTable("Batismos");

        builder.HasKey(batismo => batismo.Id);

        builder.Property(batismo => batismo.Id)
            .ValueGeneratedNever();

        builder.Property(batismo => batismo.DataBatismo)
            .IsRequired();

        builder.Property(batismo => batismo.Oficiante)
            .HasMaxLength(Batismo.TamanhoMaximoOficiante)
            .IsRequired();

        builder.Property(batismo => batismo.Local)
            .HasMaxLength(Batismo.TamanhoMaximoLocal);

        builder.HasOne<Membro>()
            .WithMany()
            .HasForeignKey(batismo => batismo.MembroId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cada membro tem no máximo um batismo.
        builder.HasIndex(batismo => batismo.MembroId)
            .IsUnique();
    }
}