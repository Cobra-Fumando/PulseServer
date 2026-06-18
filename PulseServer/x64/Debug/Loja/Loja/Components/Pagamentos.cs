using Loja.Conexao;
using Loja.Config;
using Loja.Interfaces;
using Loja.Tabelas.TabelasDto;
using Loja.TabelasContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Loja.Components
{
    public class Pagamentos
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ICarrinhoConfig carrinhoConfig;
        private readonly IMemoryCache memoryCache;
        private readonly Userid userid;
        public Pagamentos(IServiceScopeFactory serviceScopeFactory, Userid userid, ICarrinhoConfig carrinhoConfig, IMemoryCache memoryCache)
        {
            scopeFactory = serviceScopeFactory;
            this.userid = userid;
            this.carrinhoConfig = carrinhoConfig;
            this.memoryCache = memoryCache;
        }
        public async Task<TabelaProblem<string>> PagarTudoCarrinho()
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                int userId = userid.GetUserId();
                List<HistoricoPagamentos> ListaHistorico = new List<HistoricoPagamentos>();
                string cacheKey = $"carrinho_{userId}";
                if(!memoryCache.TryGetValue(cacheKey, out List<CarrinhoDto>? itens))
                {
                    var lista = await carrinhoConfig.GetCarrinho();
                    itens = lista.Dados;
                }

                using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (CarrinhoDto? produtos in itens!)
                {
                    decimal Valor = produtos.Produto.preco * produtos.quantidade;
                    HistoricoPagamentos historicoPagamentos = new HistoricoPagamentos()
                    {
                        NomeProdutos = produtos.Produto.nome,
                        UserRecebedorId = userId,
                        UserVendedorId = produtos.Produto.VendedorId,
                    };
                    historicoPagamentos.SetValor(Valor);

                    ListaHistorico.Add(historicoPagamentos);

                    //gerar pagamentos com a api do mercado pago usando o valor
                    //definir o status do pedido para pago
                    await context.HistoricoPagamentos.AddRangeAsync(historicoPagamentos);
                    await context.SaveChangesAsync();
                }

                //colocar if caso status == pago
                return StatusProblem.Ok<string>("Pago com sucesso");

            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<string>($"Erro: {ex.Message}");
            }
        }
    }
}
