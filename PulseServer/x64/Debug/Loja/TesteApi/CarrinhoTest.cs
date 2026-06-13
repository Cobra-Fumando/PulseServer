using Loja.Components;
using Loja.Conexao;
using Loja.Migrations;
using Loja.TabelasContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteApi
{
    public class CarrinhoTest
    {
        public AppDbContext Conexao()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

            return new AppDbContext(options);
        }


    }
}
