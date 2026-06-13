using Loja.TabelasContext;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.Tabelas.TabelasDto
{
    public class CarrinhoDto
    {
        public Produtos Produto { get; set; }
        public UsuarioDto Usuario { get; set; }
        public int quantidade { get; set; }
    }
}
