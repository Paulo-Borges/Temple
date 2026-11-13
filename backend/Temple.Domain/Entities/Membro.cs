using Temple.Domain.ValueObjects;

namespace Temple.Domain.Entities;

public sealed class Membro
{
    private const int TamanhoMaximoNome = 150;

    public Guid Id { get; private set; }
    public int Matricula { get; private set; }
    public string Nome { get; private set; }
    public DateOnly DataNascimento { get; private set; }
    public Endereco Endereco { get; private set; }

    public Guid IgrejaId { get; private set; }
    public Igreja Igreja { get; private set; }

    public Membro(string nome, DateOnly dataNascimento, Endereco endereco, Igreja igreja)
    {
        ArgumentNullException.ThrowIfNull(endereco);
        ArgumentNullException.ThrowIfNull(igreja);

    
        if (!igreja.Ativa)
        {
            throw new InvalidOperationException("Não é possível cadastrar membro em uma igreja inativa.");
        }
        
        Id = Guid.NewGuid();
        Nome = ValidarNome(nome);
        DataNascimento = ValidarDataNascimento(dataNascimento);
        Endereco = endereco;
        IgrejaId = igreja.Id;
        Igreja = igreja;
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
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        string nomeLimpo = nome.Trim();

        if (nomeLimpo.Length > TamanhoMaximoNome)
        {
            throw new ArgumentException($"O nome deve ter no máximo {TamanhoMaximoNome} caracteres.", nameof(nome));
        }

        return nomeLimpo;
    }

    private static DateOnly ValidarDataNascimento(DateOnly dataNascimento)
    {
        DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

        if (dataNascimento > hoje)
        {
            throw new ArgumentOutOfRangeException(nameof(dataNascimento), "A data de nascimento não pode estar no futuro.");
        }

        return dataNascimento;
    }
}
