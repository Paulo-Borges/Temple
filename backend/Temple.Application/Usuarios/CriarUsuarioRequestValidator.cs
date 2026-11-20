using FluentValidation;

namespace Temple.Application.Usuarios;

public sealed class CriarUsuarioRequestValidator : AbstractValidator<CriarUsuarioRequest>
{
    public CriarUsuarioRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Nome).NomeDoUsuario();

        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(RegrasDoUsuario.TamanhoMaximoEmail)
            .EmailAddress();

        RuleFor(request => request.Senha)
            .NotEmpty();

        RuleFor(request => request.Perfis).PerfisDoUsuario();
    }
}
