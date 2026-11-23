using FluentValidation;
using Temple.Application.Comum.Acesso;
using Temple.Application.Membros;

namespace Temple.Application.Aniversariantes;

public sealed class AniversarianteService(
    IMembroRepository membroRepository,
    ProvedorDeEscopo provedorDeEscopo,
    TimeProvider timeProvider,
    IValidator<ListarAniversariantesRequest> listarAniversariantesValidator)
{
    public async Task<IReadOnlyList<AniversarianteResponse>> ListarAsync(ListarAniversariantesRequest filtro, CancellationToken cancellationToken)
    {
        await listarAniversariantesValidator.ValidateAndThrowAsync(filtro, cancellationToken);

        DateOnly hoje = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        int mes = hoje.Month;

        if (filtro.Mes is int mesInformado)
        {
            mes = mesInformado;
        }

        EscopoDeAcesso escopo = await provedorDeEscopo.ObterAsync(cancellationToken);

        IReadOnlyList<AniversarianteDoMes> aniversariantes =
            await membroRepository.ListarAniversariantesAsync(mes, filtro.IgrejaId, escopo, cancellationToken);

        return aniversariantes
            .Select(aniversariante => ParaResponse(aniversariante, hoje.Year))
            .ToList();
    }

    private static AniversarianteResponse ParaResponse(AniversarianteDoMes aniversariante, int ano)
    {
        return new AniversarianteResponse(
            aniversariante.Id,
            aniversariante.Matricula,
            aniversariante.Nome,
            aniversariante.DataNascimento,
            aniversariante.DataNascimento.Day,
            ano - aniversariante.DataNascimento.Year,
            aniversariante.IgrejaId,
            aniversariante.NomeDaIgreja,
            aniversariante.Grupo);
    }
}