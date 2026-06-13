using Loja.Classes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteApi
{
    public class TokenVerify
    {

        [Fact]
        public void TokenNotNull()
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

            var token = new Token(configuration);

            var generatedToken = token.GenerateToken("TestUser", 1);

            Assert.False(string.IsNullOrWhiteSpace(generatedToken));
            Assert.Contains(".", generatedToken);
            Assert.Equal(3, generatedToken.Split('.').Length);
        }
    }
}
