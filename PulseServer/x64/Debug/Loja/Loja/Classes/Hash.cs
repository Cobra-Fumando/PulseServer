using Microsoft.AspNetCore.Identity;

namespace Loja.Classes
{
    public class Hash
    {
        private readonly IPasswordHasher<object> passwordHasher;
        public Hash(IPasswordHasher<object> password)
        {
            passwordHasher = password;
        }
        public string GerarCryp(string Senha)
        {
            var result = passwordHasher.HashPassword(null, Senha);
            return result;
        }

        public bool verificarHash(string Senha, string Hash)
        {
            var result = passwordHasher.VerifyHashedPassword(null, Hash, Senha);
            return result == PasswordVerificationResult.Success;
        }
    }
}
