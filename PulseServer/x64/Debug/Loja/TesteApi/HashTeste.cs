using Loja.Classes;
using Loja.Components;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace TesteApi
{
    public class HashTeste
    {
        [Fact]
        public void HashNotNull()
        {
            Hash hash = new Hash(new PasswordHasher<object>());
            string senha = "123456";
            string hashSenha = hash.GerarCryp(senha);

            Assert.NotNull(hashSenha);
        }

        [Fact]
        public void HashSenhaVerificada()
        {
            Hash hash = new Hash(new PasswordHasher<object>());
            string senha = "123456";
            string hashSenha = hash.GerarCryp(senha);
            bool resultado = hash.verificarHash(senha, hashSenha);
            Assert.True(resultado);
        }
    }
}
