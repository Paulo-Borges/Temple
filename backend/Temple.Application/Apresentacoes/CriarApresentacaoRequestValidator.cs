using FluentValidation;

namespace Temple.Application.Apresentacoes;

public sealed class CriarApresentacaoRequestValidator : AbstractValidator<CriarApresentacaoRequest>
{
    public CriarApresentacaoRequestValidator(TimeProvider timeProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.IgrejaId)
            .NotEmpty();

        RuleFor(request => request.NomeDaCrianca).NomeObrigatorio();

        RuleFor(request => request.DataNascimentoDaCrianca).AteHoje(timeProvider);

        RuleFor(request => request.DataApresentacao).AteHoje(timeProvider);

        RuleFor(request => request.Oficiante).Oficiante();

        RuleFor(request => request.NomeDoPai)
            .NomeOpcional()
            .Must((request, _) => RegrasDaApresentacao.TemAlgumDosPais(request.NomeDoPai, request.NomeDaMae))
            .WithMessage(RegrasDaApresentacao.MensagemDosPais);

        RuleFor(request => request.NomeDaMae).NomeOpcional();
    }
}