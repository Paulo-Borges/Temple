using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Infrastructure.Identidade;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.Property(usuario => usuario.Nome)
            .HasMaxLength(Usuario.TamanhoMaximoNome)
            .IsRequired();
    }
}
