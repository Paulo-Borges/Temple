using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class MembroConfiguration : IEntityTypeConfiguration<Membro>
{
    public void Configure(EntityTypeBuilder<Membro> builder)
    {
        builder.ToTable("Membros");

        builder.HasKey(membro => membro.Id);

        builder.Property(membro => membro.Id)
            .ValueGeneratedNever();

        builder.Property(membro => membro.Matricula)
            .UseIdentityColumn();

        builder.HasIndex(membro => membro.Matricula)
            .IsUnique();

        builder.Property(membro => membro.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(membro => membro.DataNascimento)
            .IsRequired();

        builder.Property(membro => membro.CargoAtual)
            .IsRequired();

        builder.Property(membro => membro.Grupo);
        
        builder.Property(membro => membro.Ativo)
            .IsRequired();


        builder.ComplexProperty(membro => membro.Endereco, EnderecoConfiguration.Configurar);

        builder.HasOne<Igreja>()
            .WithMany()
            .HasForeignKey(membro => membro.IgrejaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(membro => membro.HistoricoCargos)
            .WithOne()
            .HasForeignKey(historico => historico.MembroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(membro => membro.HistoricoCargos)
            .HasField("_historicoCargos");
    }
}
