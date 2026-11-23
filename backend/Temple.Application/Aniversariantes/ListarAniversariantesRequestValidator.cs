using FluentValidation;

namespace Temple.Application.Aniversariantes;

public sealed class ListarAniversariantesRequestValidator : AbstractValidator<ListarAniversariantesRequest>
{
    public ListarAniversariantesRequestValidator()
    {
        RuleFor(request => request.Mes)
            .InclusiveBetween(1, 12);
    }
}