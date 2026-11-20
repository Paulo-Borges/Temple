using FluentValidation;

namespace Temple.Application.Usuarios;

public sealed class VincularMembroRequestValidator : AbstractValidator<VincularMembroRequest>
{
    public VincularMembroRequestValidator()
    {
        RuleFor(request => request.MembroId)
            .NotEmpty();
    }
}
