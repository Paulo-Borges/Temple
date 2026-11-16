using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.ValueObjects;

namespace Temple.Infrastructure.Persistence.Configurations;

internal static class EnderecoConfiguration
{
    public static void Configurar(ComplexPropertyBuilder<Endereco> endereco)
    {
        endereco.Property(e => e.Logradouro)
            .HasColumnName("Logradouro")
            .HasMaxLength(150)
            .IsRequired();

        endereco.Property(e => e.Numero)
            .HasColumnName("Numero")
            .HasMaxLength(10)
            .IsRequired();

        endereco.Property(e => e.Complemento)
            .HasColumnName("Complemento")
            .HasMaxLength(100);

        endereco.Property(e => e.Bairro)
            .HasColumnName("Bairro")
            .HasMaxLength(100)
            .IsRequired();

        endereco.Property(e => e.Cidade)
            .HasColumnName("Cidade")
            .HasMaxLength(100)
            .IsRequired();

        endereco.Property(e => e.Uf)
            .HasColumnName("Uf")
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();

        endereco.Property(e => e.Cep)
            .HasColumnName("Cep")
            .HasMaxLength(8)
            .IsFixedLength()
            .IsRequired();
    }
}
