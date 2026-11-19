using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities;
using Temple.Infrastructure.Identidade;

namespace Temple.Infrastructure.Persistence;

public sealed class TempleDbContext(DbContextOptions<TempleDbContext> options)
    : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Igreja> Igrejas => Set<Igreja>();
    public DbSet<Membro> Membros => Set<Membro>();
    public DbSet<HistoricoCargo> HistoricoCargos => Set<HistoricoCargo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TempleDbContext).Assembly);
    }
}
