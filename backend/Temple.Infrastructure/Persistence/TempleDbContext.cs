using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities;

namespace Temple.Infrastructure.Persistence;

public sealed class TempleDbContext(DbContextOptions<TempleDbContext> options) : DbContext(options)
{
    public DbSet<Igreja> Igrejas => Set<Igreja>();
    public DbSet<Membro> Membros => Set<Membro>();
    public DbSet<HistoricoCargo> HistoricoCargos => Set<HistoricoCargo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TempleDbContext).Assembly);
    }
}
