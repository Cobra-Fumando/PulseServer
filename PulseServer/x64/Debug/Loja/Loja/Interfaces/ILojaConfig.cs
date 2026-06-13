using Loja.Tabelas;
using Loja.Tabelas.TabelasDto;
using Loja.TabelasContext;

namespace Loja.Interfaces
{
    public interface ILojaConfig
    {

        public Task<TabelaProblem<List<CarrinhoDto>>> GetCarrinho();
        public Task<TabelaProblem<List<Produtos>>> AddCarrinho(CarrinhoParam carrinho);
        public Task<TabelaProblem<List<Carrinho>>> RemoveCarrinho(int carrinhoId);
        public Task<TabelaProblem<List<Produtos>>> GetProdutos(int page, int TamanhoPage);
        public Task<TabelaProblem<Produtos>> GetProdutoById(int id);
        public Task<TabelaProblem<Produtos>> AddProduto(Produtos produto);


    }
}
