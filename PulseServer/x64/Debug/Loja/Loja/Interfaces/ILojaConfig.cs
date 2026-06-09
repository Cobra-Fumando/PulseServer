using Loja.TabelasContext;

namespace Loja.Interfaces
{
    public interface ILojaConfig
    {

        public Task<TabelaProblem<List<Carrinho>>> GetCarrinho();
        public Task<TabelaProblem<List<Produtos>>> AddCarrinho(Carrinho carrinho);
        public Task<TabelaProblem<List<Produtos>>> RemoveCarrinho(int carrinhoId);
        public Task<TabelaProblem<List<Produtos>>> GetProdutos(int page, int TamanhoPage);
        public Task<TabelaProblem<Produtos>> GetProdutoById(int id);
        public Task<TabelaProblem<Produtos>> AddProduto(Produtos produto);


    }
}
