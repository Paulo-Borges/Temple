namespace Temple.Application.Comum.Persistencia;

public interface IUnitOfWork
{
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
