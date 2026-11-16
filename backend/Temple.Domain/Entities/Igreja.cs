using Temple.Domain.ValueObjects;
// namespace e classe  
namespace Temple.Domain.Entities;

public sealed class Igreja
{

    // constante e lista privada para filiais
    private const int TamanhoMaximoNome = 150;
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
            throw new InvalidOperationException("Uma filial não pode ter filiais.");
        }

        Igreja filial = new(nome, cnpj, telefone, dataFundacao, ativa, endereco)
        {
            IgrejaMatrizId = Id,
            IgrejaMatriz = this
        };

        _filiais.Add(filial);
        return filial;
    }


    // métodos para ativar e desativar a igreja

        public void AlterarEndereco(Endereco novoEndereco)
    {
        ArgumentNullException.ThrowIfNull(novoEndereco);
        Endereco = novoEndereco;
    }

        public void Ativar()
    {
        Ativa = true;
    }

    public void Desativar()
    {
        Ativa = false;
    }

    // métodos de validação privadas.
        private static string ValidarNome(string nome)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        string nomeLimpo = nome.Trim();

        if (nomeLimpo.Length > TamanhoMaximoNome)
        {
            throw new ArgumentException($"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.", nameof(nome));
        }

        return nomeLimpo;
    }

    private static string ValidarCnpj(string cnpj)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cnpj);

        string semMascara = cnpj.Trim()
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .Replace("/", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant();

        if (semMascara.Length != 14)
        {
            throw new ArgumentException("O CNPJ deve ter 14 caracteres.", nameof(cnpj));
        }

        for (int i = 0; i < 12; i++)
        {
            if (!char.IsAsciiDigit(semMascara[i]) && !char.IsAsciiLetterUpper(semMascara[i]))
            {
                throw new ArgumentException("Os 12 primeiros caracteres do CNPJ devem ser letras ou números.", nameof(cnpj));
            }
        }

        if (!char.IsAsciiDigit(semMascara[12]) || !char.IsAsciiDigit(semMascara[13]))
        {
            throw new ArgumentException("Os 2 últimos caracteres do CNPJ (dígitos verificadores) devem ser números.", nameof(cnpj));
        }

        return semMascara;
    }

    private static string ValidarTelefone(string telefone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(telefone);

        string semMascara = telefone.Trim()
            .Replace("(", string.Empty, StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        if (!semMascara.All(char.IsAsciiDigit))
        {
            throw new ArgumentException("O telefone deve conter apenas números.", nameof(telefone));
        }

        if (semMascara.Length is not (10 or 11))
        {
            throw new ArgumentException("O telefone deve ter 10 ou 11 dígitos (com DDD).", nameof(telefone));
        }

        return semMascara;
    }

    private static DateOnly ValidarDataFundacao(DateOnly dataFundacao)
    {
        DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

        if (dataFundacao > hoje)
        {
            throw new ArgumentOutOfRangeException(nameof(dataFundacao), "A data de fundação não pode estar no futuro.");
        }

        return dataFundacao;
    }
}



