using Loja.Conexao;
using Loja.Config;
using Loja.TabelasContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Loja.Interfaces;
using Loja.Tabelas;
using Loja.Tabelas.TabelasDto;
using System.Data.Common;

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

        public int? GetUserId()
        {
            var tokenId = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(tokenId, out int id) ? id : null;
        }

        //---------------------------------------------------- Carrinho ----------------------------------------------------//
        public async Task<TabelaProblem<List<CarrinhoDto>>> GetCarrinho()
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null) return StatusProblem.Fail<List<CarrinhoDto>>("Usuário não autenticado");
                string cacheKey = $"carrinho_{userId}";

                if (!cache.TryGetValue(cacheKey, out List<CarrinhoDto>? list))
                {
                    list = await context.Carrinho.AsNoTracking()
                                        .Include(c => c.Produto)
                                        .Include(c => c.Usuario)
                                        .Where(c => c.UserId == userId)
                                        .Select(c => new CarrinhoDto { Produto = c.Produto, 
                                            Usuario = new UsuarioDto { email = c.Usuario.email, nome = c.Usuario.nome},
                                            quantidade = c.quantidade})
                                        .ToListAsync();

                    if (list.Count == 0) return StatusProblem.Fail<List<CarrinhoDto>>("Carrinho vazio");

                    cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));
                }

                return StatusProblem.Ok<List<CarrinhoDto>>("Carrinho encontrado com sucesso", list);
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<CarrinhoDto>>($"Erro ao obter carrinho: {ex.Message}");
            }
        }

        public async Task<TabelaProblem<List<Produtos>>> AddCarrinho(CarrinhoParam carrinho)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null) return StatusProblem.Fail<List<Produtos>>("Usuário não autenticado");

                string cacheKey = $"carrinho_{userId}";

                Carrinho carrinhoAdd = new Carrinho
                {
                    ProdutoId = carrinho.ProdutoId,
                    UserId = carrinho.UserId,
                    quantidade = carrinho.quantidade
                };

                context.Carrinho.Add(carrinhoAdd);
                await context.SaveChangesAsync();

                var list = await context.Carrinho.AsNoTracking()
                                .Include(c => c.Produto)
                                .Where(c => c.UserId == userId).ToListAsync();

                cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));

                return StatusProblem.Ok<List<Produtos>>("Produto adicionado ao carrinho com sucesso", list.Select(c => c.Produto).ToList());
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<Produtos>>("Ocorreu um erro ao tentar adicionar ao carrinho");
            }
        }

        public async Task<TabelaProblem<List<Carrinho>>> RemoveCarrinho(int carrinhoId)
        {
            try
            {
                int? userId = GetUserId();
                if (userId == null) return StatusProblem.Fail<List<Carrinho>>("Usuário não autenticado");
                string cacheKey = $"carrinho_{userId}";

                var carrinhoItem = await context.Carrinho.AsNoTracking().FirstOrDefaultAsync(c => c.id == carrinhoId && c.UserId == userId);
                if (carrinhoItem == null)
                {
                    return StatusProblem.Fail<List<Carrinho>>("Item do carrinho não encontrado");
                }

                context.Carrinho.Remove(carrinhoItem);
                await context.SaveChangesAsync();

                var list = await context.Carrinho.Where(c => c.UserId == userId).ToListAsync();

                cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));
                return StatusProblem.Ok<List<Carrinho>>("Produto removido do carrinho com sucesso", list);
            }catch (DbException ex)
            {
                return StatusProblem.Fail<List<Carrinho>>($"Ocorreu um erro no banco de dados {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<Carrinho>>($"Erro ao remover item do carrinho: {ex.Message}");
            }
        }

        //---------------------------------------------------- Produtos ----------------------------------------------------//
        public async Task<TabelaProblem<List<Produtos>>> GetProdutos(int page, int TamanhoPage)
        {
            try
            {
                if (page < 1 || TamanhoPage < 1) return StatusProblem.Fail<List<Produtos>>("Parâmetros de paginação inválidos");


                int? userId = GetUserId();
                if (userId == null) return StatusProblem.Fail<List<Produtos>>("Usuário não autenticado");
                string cacheKey = $"produtos_page_{page}_size_{TamanhoPage}";

                if (!cache.TryGetValue(cacheKey, out List<Produtos>? produtos))
                {
                    produtos = await context.Produtos.AsNoTracking().Skip((page - 1) * TamanhoPage).Take(TamanhoPage).ToListAsync();
                    if (produtos.Count == 0) return StatusProblem.Fail<List<Produtos>>("Nenhum produto encontrado");

                    cache.Set(cacheKey, produtos, TimeSpan.FromMinutes(5));
                }

                return StatusProblem.Ok<List<Produtos>>("Produtos encontrados com sucesso", produtos);
            }catch (DbException ex)
            {
                return StatusProblem.Fail<List<Produtos>>($"Ocorreu um erro no banco de dados {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<Produtos>>($"Erro ao obter produtos: {ex.Message}");
            }
        }
        public async Task<TabelaProblem<Produtos>> GetProdutoById(int id)
        {
            try
            {
                var produto = await context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.id == id);
                if (produto == null) return StatusProblem.Fail<Produtos>("Produto não encontrado");
                return StatusProblem.Ok<Produtos>("Produto encontrado com sucesso", produto);
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<Produtos>($"Erro ao obter produto: {ex.Message}");
            }

        }

        public async Task<TabelaProblem<Produtos>> AddProduto(Produtos produto)
        {
            //colocar o rebbitMQ para enviar uma mensagem de atualização de cache para os consumidores do serviço de produtos
            //colocar redis para atualizar o cache de produtos
            try
            {
                context.Produtos.Add(produto);
                await context.SaveChangesAsync();
                return StatusProblem.Ok<Produtos>("Produto adicionado com sucesso", produto);
            } catch (Exception ex)
            {
                return StatusProblem.Fail<Produtos>($"Erro ao adicionar produto: {ex.Message}");
            }
        }
    }
}
