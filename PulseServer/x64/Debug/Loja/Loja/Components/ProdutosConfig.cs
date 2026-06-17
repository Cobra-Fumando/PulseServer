using Loja.Config;
using Loja.TabelasContext;
using System.Data.Common;
using Loja.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Loja.Conexao;
using Microsoft.EntityFrameworkCore;
using Loja.Classes;
using Loja.Tabelas;

namespace Loja.Components
{
    public class ProdutosConfig : IProdutosConfig
    {
        private readonly AppDbContext context;
        private readonly IMemoryCache cache;
        private readonly Userid userid;
        private readonly TriggerEmail triggerEmail;
        private readonly ILogger<ProdutosConfig> logger;
        private readonly IConfiguration configuration;
        public ProdutosConfig(AppDbContext context, IMemoryCache cache, Userid userid, TriggerEmail triggerEmail, ILogger<ProdutosConfig> logger, IConfiguration configuration)
        {
            this.context = context;
            this.cache = cache;
            this.userid = userid;
            this.triggerEmail = triggerEmail;
            this.logger = logger;
            this.configuration = configuration;
        }

        //---------------------------------------------------- Produtos ----------------------------------------------------//
        public async Task<TabelaProblem<List<Produtos>>> GetProdutos(int page, int TamanhoPage, bool Ordem)
        {
            try
            {
                if (page < 1 || TamanhoPage < 1) return StatusProblem.Fail<List<Produtos>>("Parâmetros de paginação inválidos");


                int? userId = userid.GetUserId();
                if (userId == null) return StatusProblem.Fail<List<Produtos>>("Usuário não autenticado");
                string cacheKey = $"produtos_page_{page}_size_{TamanhoPage}_Ordem{Ordem}";

                if (!cache.TryGetValue(cacheKey, out List<Produtos>? produtos))
                {
                    if (Ordem)
                    {
                        produtos = await context.Produtos.AsNoTracking().Skip((page - 1) * TamanhoPage).Take(TamanhoPage).ToListAsync();
                    }
                    else
                    {
                        produtos = await context.Produtos.AsNoTracking().OrderByDescending(p => p.id).Skip((page - 1) * TamanhoPage).Take(TamanhoPage).ToListAsync();
                    }

                    if (produtos.Count == 0) return StatusProblem.Fail<List<Produtos>>("Nenhum produto encontrado");

                    cache.Set(cacheKey, produtos, TimeSpan.FromMinutes(5));
                }

                return StatusProblem.Ok<List<Produtos>>("Produtos encontrados com sucesso", produtos);
            }
            catch (DbException ex)
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
            }catch (DbUpdateException ex)
            {
                return StatusProblem.Fail<Produtos>($"Ocorreu um erro no banco de dados {ex.Message}");
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
            }catch (DbUpdateException ex)
            {
                return StatusProblem.Fail<Produtos>($"Ocorreu um erro no banco de dados {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<Produtos>($"Erro ao adicionar produto: {ex.Message}");
            }
        }

        public async Task<TabelaProblem<List<Produtos>>> SearchProductByName(string Name)
        {
            try
            {

                var list = await context.Produtos.AsNoTracking().Where(p => p.nome.Contains(Name)).ToListAsync();
                if (list.Count == 0)
                {
                    return StatusProblem.Fail<List<Produtos>>("Nenhum produto encontrado com esse nome");
                }

                return StatusProblem.Ok<List<Produtos>>("Produtos encontrados com sucesso", list);
            }
            catch (DbException ex)
            {
                return StatusProblem.Fail<List<Produtos>>($"Ocorreu um erro no banco de dados {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<List<Produtos>>($"Erro ao buscar produto: {ex.Message}");
            }
        }

        public async Task<TabelaProblem<string>> EnviarEmail()
        {
            var email = new EmailInformation
            {
                EmailRemetente = configuration["EmailTeste:EmailRemetente"]!,
                Assunto = configuration["EmailTeste:Assunto"]!,
                Destinatario = configuration["EmailTeste:Destinatario"]!,
                EmailDestinatario = configuration["EmailTeste:EmailDestinatario"]!,
                Mensagem = configuration["EmailTeste:Mensagem"]!,
                Remetente = configuration["EmailTeste:Remetente"]!
            };
            await triggerEmail.Enviar(email);
            logger.LogInformation("EnviarEmail");
            return StatusProblem.Ok<string>("Email enviado com sucesso", "Email enviado com sucesso");
        }
    }
}
