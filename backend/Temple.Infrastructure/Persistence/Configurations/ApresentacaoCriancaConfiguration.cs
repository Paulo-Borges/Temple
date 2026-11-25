using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence.Configurations;

internal sealed class ApresentacaoCriancaConfiguration : IEntityTypeConfiguration<ApresentacaoCrianca>
{
    public void Configure(EntityTypeBuilder<ApresentacaoCrianca> builder)
    {
        builder.ToTable("ApresentacoesCriancas");

        builder.HasKey(apresentacao => apresentacao.Id);

        builder.Property(apresentacao => apresentacao.Id)
            .ValueGeneratedNever();

        builder.Property(apresentacao => apresentacao.NomeDaCrianca)
            .HasMaxLength(ApresentacaoCrianca.TamanhoMaximoNome)
            .IsRequired();

        builder.Property(apresentacao => apresentacao.DataNascimentoDaCrianca)
            .IsRequired();

        builder.Property(apresentacao => apresentacao.NomeDoPai)
            .HasMaxLength(ApresentacaoCrianca.TamanhoMaximoNome);

        builder.Property(apresentacao => apresentacao.NomeDaMae)
            .HasMaxLength(ApresentacaoCrianca.TamanhoMaximoNome);

        builder.Property(apresentacao => apresentacao.DataApresentacao)
            .IsRequired();

        builder.Property(apresentacao => apresentacao.Oficiante)
            .HasMaxLength(ApresentacaoCrianca.TamanhoMaximoOficiante)
            .IsRequired();

        builder.HasOne<Igreja>()
            .WithMany()
            .HasForeignKey(apresentacao => apresentacao.IgrejaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}