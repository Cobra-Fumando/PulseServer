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
    public class CarrinhoConfig : ICarrinhoConfig
    {
        private readonly AppDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache cache;
        private readonly Userid userid;
        public CarrinhoConfig(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMemoryCache cache, Userid userid)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
            this.userid = userid;
        }
        //---------------------------------------------------- Carrinho ----------------------------------------------------//
        public async Task<TabelaProblem<List<CarrinhoDto>>> GetCarrinho()
        {
            try
            {
                int? userId = userid.GetUserId();
                if (userId == null) return StatusProblem.Fail<List<CarrinhoDto>>("Usuário não autenticado");
                string cacheKey = $"carrinho_{userId}";

                if (!cache.TryGetValue(cacheKey, out List<CarrinhoDto>? list))
                {
                    list = await context.Carrinho.AsNoTracking()
                                        .Include(c => c.Produto)
                                        .Include(c => c.Usuario)
                                        .Where(c => c.UserId == userId)
                                        .Select(c => new CarrinhoDto
                                        {
                                            Produto = c.Produto,
                                            Usuario = new UsuarioDto { email = c.Usuario.email, nome = c.Usuario.nome },
                                            quantidade = c.quantidade
                                        })
                                        .ToListAsync();

                    if (list.Count == 0) return StatusProblem.Fail<List<CarrinhoDto>>("Carrinho vazio");

                    cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));
                }

                return StatusProblem.Ok<List<CarrinhoDto>>("Carrinho encontrado com sucesso", list);
            }
            catch (DbException ex)
            {
                return StatusProblem.Fail<List<CarrinhoDto>>($"Ocorreu um erro no banco de dados {ex.Message}");
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
                int? userId = userid.GetUserId();
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
            catch (DbUpdateException ex)
            {
                return StatusProblem.Fail<List<Produtos>>($"Ocorreu um erro no banco de dados {ex.Message}");
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
                int? userId = userid.GetUserId();
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
            }
            catch (DbUpdateException ex)
            {
                return StatusProblem.Fail<List<Carrinho>>($"Ocorreu um erro no banco de dados {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<Carrinho>>($"Erro ao remover item do carrinho: {ex.Message}");
            }
        }

        public async Task<TabelaProblem<List<Carrinho>>> UpdateCarrinho(int carrinhoId, int quantidade)
        {
            try
            {
                int? userId = userid.GetUserId();
                if (userId == null) return StatusProblem.Fail<List<Carrinho>>("Usuário não autenticado");
                string cacheKey = $"carrinho_{userId}";
                var carrinhoItem = await context.Carrinho.FirstOrDefaultAsync(c => c.id == carrinhoId && c.UserId == userId);
                if (carrinhoItem == null)
                {
                    return StatusProblem.Fail<List<Carrinho>>("Item do carrinho não encontrado");
                }
                carrinhoItem.quantidade = quantidade;
                context.Carrinho.Update(carrinhoItem);
                await context.SaveChangesAsync();
                var list = await context.Carrinho.Where(c => c.UserId == userId).ToListAsync();
                cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));
                return StatusProblem.Ok<List<Carrinho>>("Produto atualizado no carrinho com sucesso", list);
            }
            catch (DbUpdateException ex)
            {
                return StatusProblem.Fail<List<Carrinho>>($"Ocorreu um erro no banco de dados {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<Carrinho>>($"Erro ao atualizar item do carrinho: {ex.Message}");
            }
        }
    }
}
