using Microsoft.AspNetCore.Identity;

namespace Temple.Infrastructure.Identidade;

internal sealed class DescritorDeErrosEmPortugues : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
    {
        return Erro(nameof(PasswordTooShort), $"A senha deve ter pelo menos {length} caracteres.");
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return Erro(nameof(PasswordRequiresNonAlphanumeric), "A senha deve ter pelo menos um símbolo (por exemplo: @, #, !).");
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return Erro(nameof(PasswordRequiresDigit), "A senha deve ter pelo menos um número (0-9).");
    }

    public override IdentityError PasswordRequiresLower()
    {
        return Erro(nameof(PasswordRequiresLower), "A senha deve ter pelo menos uma letra minúscula (a-z).");
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return Erro(nameof(PasswordRequiresUpper), "A senha deve ter pelo menos uma letra maiúscula (A-Z).");
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return Erro(nameof(PasswordRequiresUniqueChars), $"A senha deve ter pelo menos {uniqueChars} caracteres diferentes.");
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return Erro(nameof(DuplicateEmail), $"O e-mail '{email}' já está em uso.");
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return Erro(nameof(DuplicateUserName), $"O usuário '{userName}' já existe.");
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return Erro(nameof(InvalidEmail), $"O e-mail '{email}' é inválido.");
    }

    public override IdentityError InvalidUserName(string? userName)
    {
        return Erro(nameof(InvalidUserName), $"O usuário '{userName}' é inválido. Use apenas letras, números e os símbolos - . _ @ +");
    }

    private static IdentityError Erro(string codigo, string descricao)
    {
        return new IdentityError
        {
            Code = codigo,
            Description = descricao
        };
    }
}
