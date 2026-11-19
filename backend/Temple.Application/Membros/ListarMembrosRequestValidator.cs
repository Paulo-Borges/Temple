using FluentValidation;
using Temple.Domain.Entities;

namespace Temple.Application.Membros;

public sealed class ListarMembrosRequestValidator : AbstractValidator<ListarMembrosRequest>
{
    private const int TamanhoMaximoPagina = 100;

    public ListarMembrosRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Nome)
            .MaximumLength(Membro.TamanhoMaximoNome);

        RuleFor(request => request.Cargo)
            .IsInEnum();

        RuleFor(request => request.Pagina)
            .GreaterThanOrEqualTo(1);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, TamanhoMaximoPagina);
    }
}
