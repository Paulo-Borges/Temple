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

    public bool Ativo { get; private set; }


    public Guid IgrejaId { get; private set; }
    // public Igreja Igreja { get; private set; }
    public IReadOnlyList<HistoricoCargo> HistoricoCargos => _historicoCargos.AsReadOnly();

    // Construtor usado apenas pelo EF Core ao ler do banco.
    // O Endereco é preenchido pelo EF logo após a criação do objeto.
#pragma warning disable CS8618
     private Membro(Guid id, int matricula, string nome, DateOnly dataNascimento, TipoCargo cargoAtual, GrupoMembro? grupo, bool ativo, Guid igrejaId)
#pragma warning restore CS8618
    {
        Id = id;
        Matricula = matricula;
        Nome = nome;
        DataNascimento = dataNascimento;
        CargoAtual = cargoAtual;
        Grupo = grupo;
        Ativo = ativo;
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
                Ativo = true;

    }

        public void AlterarGrupo(GrupoMembro? novoGrupo)
    {
        Grupo = ValidarGrupo(novoGrupo);
    }

      public void PromoverCargo(
        TipoCargo novoCargo,
        DateOnly dataPromocao,
        string? observacao,
        Guid promovidoPorId,
        string promovidoPorNome,
        DateTimeOffset agora)
    {
        if (!Ativo)
        {
            throw new DomainException("Não é possível promover um membro desativado.");
        }

        ValidarCargo(novoCargo);

        if (novoCargo == CargoAtual)
        {
            throw new DomainException("O novo cargo deve ser diferente do cargo atual.");
        }

        DateOnly hoje = DateOnly.FromDateTime(agora.DateTime);

        if (dataPromocao > hoje)
        {
            throw new DomainException("A data da promoção não pode estar no futuro.");
        }

        if (dataPromocao < DataNascimento)
        {
            throw new DomainException("A data da promoção não pode ser anterior ao nascimento do membro.");
        }

        if (_historicoCargos.Count > 0)
        {
            DateOnly ultimaPromocao = _historicoCargos.Max(historico => historico.DataPromocao);

            if (dataPromocao < ultimaPromocao)
            {
                throw new DomainException($"A data da promoção não pode ser anterior à última promoção, de {ultimaPromocao:dd/MM/yyyy}.");
            }
        }

        if (novoCargo < CargoAtual && string.IsNullOrWhiteSpace(observacao))
        {
            throw new DomainException("Para rebaixar o cargo, informe o motivo na observação.");
        }

        HistoricoCargo historico = new(Id, CargoAtual, novoCargo, dataPromocao, observacao, promovidoPorId, promovidoPorNome, agora);
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

        public void Ativar(Igreja igreja)
    {
        ArgumentNullException.ThrowIfNull(igreja);

        if (igreja.Id != IgrejaId)
        {
            throw new InvalidOperationException("A igreja informada não é a igreja do membro.");
        }

        if (!igreja.Ativa)
        {
            throw new DomainException("Não é possível ativar um membro de uma igreja inativa.");
        }

        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
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
