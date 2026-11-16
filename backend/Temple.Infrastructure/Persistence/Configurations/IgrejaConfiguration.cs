using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class IgrejaConfiguration : IEntityTypeConfiguration<Igreja>
{
    public void Configure(EntityTypeBuilder<Igreja> builder)
    {
        builder.ToTable("Igrejas");

        builder.HasKey(igreja => igreja.Id);

        builder.Property(igreja => igreja.Id)
            .ValueGeneratedNever();

        builder.Property(igreja => igreja.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(igreja => igreja.Cnpj)
            .HasMaxLength(14)
            .IsFixedLength()
            .IsRequired();

        builder.HasIndex(igreja => igreja.Cnpj)
            .IsUnique();

        builder.Property(igreja => igreja.Telefone)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(igreja => igreja.DataFundacao)
            .IsRequired();

        builder.Property(igreja => igreja.Ativa)
            .IsRequired();

        builder.ComplexProperty(igreja => igreja.Endereco, EnderecoConfiguration.Configurar);

        builder.Ignore(igreja => igreja.EhMatriz);
    }
}
