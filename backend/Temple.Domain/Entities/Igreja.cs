using Temple.Domain.Exceptions;
using Temple.Domain.ValueObjects;
// namespace e classe  
namespace Temple.Domain.Entities;

public sealed class Igreja
{

    // constante e lista privada para filiais
    public const int TamanhoMaximoNome = 150;
    private readonly List<Igreja> _filiais = [];

    // propriedades públicas da igreja
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Cnpj { get; private set; }

    public string Telefone { get; private set; }

    public DateOnly DataFundacao { get; private set; }

    public bool Ativa { get; private set; }
    public Endereco Endereco { get; private set; }


    public Guid? IgrejaMatrizId { get; private set; }
    public Igreja? IgrejaMatriz { get; private set; }
    public IReadOnlyList<Igreja> Filiais => _filiais.AsReadOnly();


    // indica se a igreja é matriz
    public bool EhMatriz => IgrejaMatrizId == null;

        // Construtor usado apenas pelo EF Core ao ler do banco.
    // O Endereco é preenchido pelo EF logo após a criação do objeto.
#pragma warning disable CS8618
    private Igreja(Guid id, string nome, string cnpj, string telefone, DateOnly dataFundacao, bool ativa, Guid? igrejaMatrizId)
#pragma warning restore CS8618
    {
        Id = id;
        Nome = nome;
        Cnpj = cnpj;
        Telefone = telefone;
        DataFundacao = dataFundacao;
        Ativa = ativa;
        IgrejaMatrizId = igrejaMatrizId;
    }



    // construtor privado da igreja

        private Igreja(string nome, string cnpj, string telefone, DateOnly dataFundacao, bool ativa, Endereco endereco)
    {

        ArgumentNullException.ThrowIfNull(endereco);

        Id = Guid.NewGuid();
        Nome = ValidarNome(nome);
        Cnpj = ValidarCnpj(cnpj);
        Telefone = ValidarTelefone(telefone);
        DataFundacao = ValidarDataFundacao(dataFundacao);
        Ativa = ativa;
        Endereco = endereco;
    }


    // método estático para criar matriz
    public static Igreja CriarMatriz(string nome, string cnpj, string telefone, DateOnly dataFundacao, bool ativa,  Endereco endereco)
    {
        return new Igreja(nome, cnpj, telefone, dataFundacao, ativa, endereco);
    }

    // método para criar filial
    public Igreja CriarFilial(string nome, string cnpj, string telefone, DateOnly dataFundacao, bool ativa,  Endereco endereco)
    {
         if (!EhMatriz)
        {
            throw new DomainException("Uma filial não pode ter filiais.");
        }

                if (ativa && !Ativa)
        {
            throw new DomainException("Não é possível criar uma filial ativa em uma matriz desativada.");
        }

        Igreja filial = new(nome, cnpj, telefone, dataFundacao, ativa, endereco)
        {
            IgrejaMatrizId = Id,
            IgrejaMatriz = this
        };

        _filiais.Add(filial);
        return filial;
    }

        public void AlterarNome(string novoNome)
    {
        Nome = ValidarNome(novoNome);
    }

    public void AlterarTelefone(string novoTelefone)
    {
        Telefone = ValidarTelefone(novoTelefone);
    }


    // métodos para ativar e desativar a igreja

        public void AlterarEndereco(Endereco novoEndereco)
    {
        ArgumentNullException.ThrowIfNull(novoEndereco);
        Endereco = novoEndereco;
    }

      public void Ativar()
    {
        if (!EhMatriz)
        {
            if (IgrejaMatriz is null)
            {
                throw new InvalidOperationException("A matriz da filial precisa estar carregada para ativá-la.");
            }

            if (!IgrejaMatriz.Ativa)
            {
                throw new DomainException("A matriz desta filial está desativada.");
            }
        }

        Ativa = true;
    }

    public void Desativar()
    {
        if (_filiais.Any(filial => filial.Ativa))
        {
            throw new DomainException("Desative as filiais antes da matriz.");
        }

        Ativa = false;
    }


    // métodos de validação privadas.
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

    private static string ValidarCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            throw new DomainException("O CNPJ é obrigatório.");
        }

        string semMascara = cnpj.Trim()
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .Replace("/", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

        if (semMascara.Length != 14)
        {
            throw new DomainException("O CNPJ deve ter 14 caracteres.");
        }

        for (int i = 0; i < 12; i++)
        {
            if (!char.IsAsciiDigit(semMascara[i]) && !char.IsAsciiLetterUpper(semMascara[i]))
            {
                throw new DomainException("Os 12 primeiros caracteres do CNPJ devem ser letras ou números.");
            }
        }

        if (!char.IsAsciiDigit(semMascara[12]) || !char.IsAsciiDigit(semMascara[13]))
        {
            throw new DomainException("Os 2 últimos caracteres do CNPJ (dígitos verificadores) devem ser números.");
        }

        return semMascara;
    }

    private static string ValidarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new DomainException("O telefone é obrigatório.");
        }

        string semMascara = telefone.Trim()
            .Replace("(", string.Empty, StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        if (!semMascara.All(char.IsAsciiDigit))
        {
            throw new DomainException("O telefone deve conter apenas números.");
        }

        if (semMascara.Length is not (10 or 11))
        {
            throw new DomainException("O telefone deve ter 10 ou 11 dígitos (com DDD).");
        }

        return semMascara;
    }

    private static DateOnly ValidarDataFundacao(DateOnly dataFundacao)
    {
        DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

        if (dataFundacao > hoje)
        {
            throw new DomainException("A data de fundação não pode estar no futuro.");
        }

        return dataFundacao;
    }
}



