using Loja.TabelasContext;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.Tabelas
{
    public class CarrinhoParam
    {
        public int ProdutoId { get; set; }
        public int UserId { get; set; }
        public int quantidade { get; set; }
    }
}
