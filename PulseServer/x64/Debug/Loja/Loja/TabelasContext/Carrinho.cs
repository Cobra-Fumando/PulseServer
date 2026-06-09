using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.TabelasContext
{
    [Table("Carrinho")]
    public class Carrinho
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Produto")]
        public int ProdutoId { get; set; }
        public Produtos Produto { get; set; }

        [ForeignKey("Usuario")]
        public int UserId { get; set; }
        public Users Usuario { get; set; }
        public int quantidade { get; set; }
    }
}
