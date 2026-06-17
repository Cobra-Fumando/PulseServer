using Loja.TabelasContext;

namespace Loja.Interfaces
{
    public interface IProdutosConfig
    {
        public Task<TabelaProblem<List<Produtos>>> GetProdutos(int page, int TamanhoPage, bool Ordem);
        public Task<TabelaProblem<Produtos>> GetProdutoById(int id);
        public Task<TabelaProblem<Produtos>> AddProduto(Produtos produto);
        public Task<TabelaProblem<List<Produtos>>> SearchProductByName(string Name);
        public Task<TabelaProblem<string>> EnviarEmail();
    }
}
