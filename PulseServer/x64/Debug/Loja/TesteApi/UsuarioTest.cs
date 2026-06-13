using Loja.Classes;
using Loja.Components;
using Loja.Conexao;
using Loja.Tabelas;
using Loja.TabelasContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteApi
{
    public class UsuarioTest
    {
        private AppDbContext CriarContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

            return new AppDbContext(options);
        }

        private IConfiguration ConfigMemory()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "JwtSettings:Secret", "THIS_IS_A_SUPER_SECRET_KEY_123456" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" }
            };


            IConfiguration configuration = new ConfigurationBuilder()
                                                .AddInMemoryCollection(inMemorySettings)
                                                .Build();

            return configuration;
        }

        private LoginClass CriarService()
        {
            IConfiguration configuration = ConfigMemory();
            var context = CriarContext();

            var token = new Token(configuration);
            var hash = new Hash(new PasswordHasher<object>());
            return new LoginClass(context, token, hash);
        }

        [Fact]
        public async Task TestarLoginVazioUsuario()
        {
            var authService = CriarService();

            var user = new UserLogin
            {
                email = "",
                senha = ""
            };

            var result = await authService.Login(user);
            Assert.False(result.Sucesso);
        }

        [Fact]
        public async Task TestarLoginValidoUsuario()
        {
            var authService = CriarService();

            Users users = new Users
            {
                id = 1,
                email = "ronaldo083@gmail.com",
                nome = "Ronaldo",
                senha = "123456"
            };

            var result = await authService.Cadastrar(users);
            Assert.True(result.Sucesso);

            var user = new UserLogin
            {
                email = "ronaldo083@gmail.com",
                senha = "123456"
            };

            var resultLogin = await authService.Login(user);
            Assert.True(resultLogin.Sucesso);

        }

        [Fact]
        public async Task TestarCadastroUsuario()
        {
            var auth = CriarService();

            Users users = new Users
            {
                id = 1,
                email = "ronaldo083@gmail.com",
                nome = "Ronaldo",
                senha = "123456"
            };

            var result = await auth.Cadastrar(users);
            Assert.True(result.Sucesso);
        }
    }
}
