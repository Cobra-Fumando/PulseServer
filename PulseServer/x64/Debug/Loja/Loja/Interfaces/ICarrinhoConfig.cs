using Loja.Tabelas;
using Loja.Tabelas.TabelasDto;
using Loja.TabelasContext;

namespace Loja.Interfaces
{
    public interface ICarrinhoConfig
    {

        public Task<TabelaProblem<List<CarrinhoDto>>> GetCarrinho();
        public Task<TabelaProblem<List<Produtos>>> AddCarrinho(CarrinhoParam carrinho);
        public Task<TabelaProblem<List<Carrinho>>> RemoveCarrinho(int carrinhoId);

    }
}
