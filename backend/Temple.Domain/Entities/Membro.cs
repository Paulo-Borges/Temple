using Temple.Domain.Enums;
using Temple.Domain.Exceptions;
using Temple.Domain.ValueObjects;

namespace Temple.Domain.Entities;

public sealed class Membro
{
    public const int TamanhoMaximoNome = 150;
    private readonly List<HistoricoCargo> _historicoCargos = [];

    public Guid Id { get; private set; }
    public int Matricula { get; private set; }
    public string Nome { get; private set; }
    public DateOnly DataNascimento { get; private set; }
    public Endereco Endereco { get; private set; }

    public TipoCargo CargoAtual { get; private set; }
    public GrupoMembro? Grupo { get; private set; }

    public Guid IgrejaId { get; private set; }
    // public Igreja Igreja { get; private set; }
    public IReadOnlyList<HistoricoCargo> HistoricoCargos => _historicoCargos.AsReadOnly();

    // Construtor usado apenas pelo EF Core ao ler do banco.
    // O Endereco é preenchido pelo EF logo após a criação do objeto.
#pragma warning disable CS8618
    private Membro(Guid id, int matricula, string nome, DateOnly dataNascimento, TipoCargo cargoAtual, GrupoMembro? grupo, Guid igrejaId)
#pragma warning restore CS8618
    {
        Id = id;
        Matricula = matricula;
        Nome = nome;
        DataNascimento = dataNascimento;
        CargoAtual = cargoAtual;
        Grupo = grupo;
        IgrejaId = igrejaId;
    }



    public Membro(string nome, DateOnly dataNascimento, Endereco endereco, Igreja igreja, TipoCargo cargoAtual, GrupoMembro? grupo )
    {
        ArgumentNullException.ThrowIfNull(endereco);
        ArgumentNullException.ThrowIfNull(igreja);

    
        if (!igreja.Ativa)
        {
            throw new DomainException("Não é possível cadastrar membro em uma igreja inativa.");
        }
        
        Id = Guid.NewGuid();
        Nome = ValidarNome(nome);
        DataNascimento = ValidarDataNascimento(dataNascimento);
        Endereco = endereco;
        IgrejaId = igreja.Id;
        // Igreja = igreja;
        CargoAtual = ValidarCargo(cargoAtual);
        Grupo = ValidarGrupo(grupo);
    }

        public void AlterarGrupo(GrupoMembro? novoGrupo)
    {
        Grupo = ValidarGrupo(novoGrupo);
    }

    public void PromoverCargo(TipoCargo novoCargo, DateOnly dataPromocao)
    {
        ValidarCargo(novoCargo);

        if (novoCargo == CargoAtual)
        {
            throw new DomainException("O novo cargo deve ser diferente do cargo atual.");
        }

        DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

        if (dataPromocao > hoje)
        {
            throw new DomainException("A data da promoção não pode estar no futuro.");
        }

        HistoricoCargo historico = new(Id, CargoAtual, novoCargo, dataPromocao);
        _historicoCargos.Add(historico);

        CargoAtual = novoCargo;
    }


    public int CalcularIdade(DateOnly dataReferencia)
    {
        int idade = dataReferencia.Year - DataNascimento.Year;

        if (dataReferencia < DataNascimento.AddYears(idade))
        {
            idade--;
        }

        return idade;
    }

    public void AlterarNome(string novoNome)
    {
        Nome = ValidarNome(novoNome);
    }

    public void AlterarEndereco(Endereco novoEndereco)
    {
        ArgumentNullException.ThrowIfNull(novoEndereco);
        Endereco = novoEndereco;
    }

    private static string ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome é obrigatório.");
        }

        string nomeLimpo = nome.Trim();

        if (nomeLimpo.Length > TamanhoMaximoNome)
        {
            throw new DomainException($"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.");
        }

        return nomeLimpo;
    }

        private static TipoCargo ValidarCargo(TipoCargo cargo)
    {
        if (!Enum.IsDefined(cargo))
        {
            throw new DomainException("Cargo inválido.");
        }

        return cargo;
    }

    private static GrupoMembro? ValidarGrupo(GrupoMembro? grupo)
    {
        if (grupo is GrupoMembro valor && !Enum.IsDefined(valor))
        {
            throw new DomainException("Grupo inválido.");
        }

        return grupo;
    }


    private static DateOnly ValidarDataNascimento(DateOnly dataNascimento)
    {
        DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

        if (dataNascimento > hoje)
        {
            throw new DomainException("A data de nascimento não pode estar no futuro.");
        }

        return dataNascimento;
    }
}
