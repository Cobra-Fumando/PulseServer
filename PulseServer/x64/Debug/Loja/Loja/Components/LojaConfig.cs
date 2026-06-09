using Loja.Conexao;
using Loja.Config;
using Loja.TabelasContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Loja.Interfaces;

namespace Loja.Components
{
    public class LojaConfig : ILojaConfig
    {
        private readonly AppDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache cache;
        public LojaConfig(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
        }

        //---------------------------------------------------- Carrinho ----------------------------------------------------//
        public async Task<TabelaProblem<List<Carrinho>>> GetCarrinho()
        {
            var tokenId = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (tokenId == null)
            {
                return StatusProblem.Fail<List<Carrinho>>("Usuário não autenticado");
            }

            int userId = int.Parse(tokenId);
            string cacheKey = $"carrinho_{userId}";

            if (!cache.TryGetValue(cacheKey, out List<Carrinho>? list))
            {
                list = await context.Carrinho.Where(c => c.UserId == userId).ToListAsync();
                if (list.Count == 0) return StatusProblem.Fail<List<Carrinho>>("Carrinho vazio");

                cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));
            }

            return StatusProblem.Ok<List<Carrinho>>("Carrinho encontrado com sucesso", list);
        }

        public async Task<TabelaProblem<List<Produtos>>> AddCarrinho(Carrinho carrinho)
        {
            var tokenId = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (tokenId == null)
            {
                return StatusProblem.Fail<List<Produtos>>("Usuário não autenticado");
            }

            int userId = int.Parse(tokenId);
            string cacheKey = $"carrinho_{userId}";

            context.Carrinho.Add(carrinho);
            await context.SaveChangesAsync();

            var list = await context.Carrinho.Where(c => c.UserId == userId).ToListAsync();

            cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));

            return StatusProblem.Ok<List<Produtos>>("Produto adicionado ao carrinho com sucesso", list.Select(c => c.Produto).ToList());
        }

        public async Task<TabelaProblem<List<Produtos>>> RemoveCarrinho(int carrinhoId)
        {
            var tokenId = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (tokenId == null)
            {
                return StatusProblem.Fail<List<Produtos>>("Usuário não autenticado");
            }

            int userId = int.Parse(tokenId);
            string cacheKey = $"carrinho_{userId}";

            var carrinhoItem = await context.Carrinho.FirstOrDefaultAsync(c => c.id == carrinhoId && c.UserId == userId);
            if (carrinhoItem == null)
            {
                return StatusProblem.Fail<List<Produtos>>("Item do carrinho não encontrado");
            }

            context.Carrinho.Remove(carrinhoItem);
            await context.SaveChangesAsync();

            var list = await context.Carrinho.Where(c => c.UserId == userId).ToListAsync();
            cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));

            return StatusProblem.Ok<List<Produtos>>("Produto removido do carrinho com sucesso", list.Select(c => c.Produto).ToList());
        }

        //---------------------------------------------------- Produtos ----------------------------------------------------//
        public async Task<TabelaProblem<List<Produtos>>> GetProdutos(int page, int TamanhoPage)
        {
            var tokenId = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (tokenId == null)
            {
                return StatusProblem.Fail<List<Produtos>>("Usuário não autenticado");
            }

            int userId = int.Parse(tokenId);
            string cacheKey = $"produtos_page_{page}_size_{TamanhoPage}_UserId{userId}";

            if (!cache.TryGetValue(cacheKey, out List<Produtos>? produtos))
            {
                produtos = await context.Produtos.Skip((page - 1) * TamanhoPage).Take(TamanhoPage).ToListAsync();
                if (produtos.Count == 0) return StatusProblem.Fail<List<Produtos>>("Nenhum produto encontrado");

                cache.Set(cacheKey, produtos, TimeSpan.FromMinutes(5));
            }

            return StatusProblem.Ok<List<Produtos>>("Produtos encontrados com sucesso", produtos);
        }
        public async Task<TabelaProblem<Produtos>> GetProdutoById(int id)
        {
            var produto = await context.Produtos.FirstOrDefaultAsync(p => p.id == id);
            if (produto == null) return StatusProblem.Fail<Produtos>("Produto não encontrado");
            return StatusProblem.Ok<Produtos>("Produto encontrado com sucesso", produto);
        }

        public async Task<TabelaProblem<Produtos>> AddProduto(Produtos produto)
        {
            //colocar o rebbitMQ para enviar uma mensagem de atualização de cache para os consumidores do serviço de produtos
            //colocar redis para atualizar o cache de produtos

            context.Produtos.Add(produto);
            await context.SaveChangesAsync();
            return StatusProblem.Ok<Produtos>("Produto adicionado com sucesso", produto);
        }
    }
}
