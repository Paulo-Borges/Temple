using Microsoft.AspNetCore.Identity;
using Temple.Application.Usuarios;

namespace Temple.Infrastructure.Identidade;

public sealed class Usuario : IdentityUser<Guid>
{
    public const int TamanhoMaximoNome = RegrasDoUsuario.TamanhoMaximoNome;

    public string Nome { get; private set; }

    public Guid? MembroId { get; private set; }

    // Construtor usado apenas pelo EF Core ao ler do banco.
    private Usuario(string nome)
    {
        Nome = nome;
    }

    public Usuario(string nome, string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        UserName = email.Trim();
        Email = email.Trim();
    }

    public void VincularMembro(Guid membroId)
    {
        if (membroId == Guid.Empty)
        {
            throw new ArgumentException("O membro do usuário é obrigatório.", nameof(membroId));
        }

        MembroId = membroId;
    }
}

