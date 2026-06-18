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
        public int quantidade { get; private set; }
        public void AdicionarQtd(int Qtd)
        {
            if(Qtd < 0)
            {
                throw new ArgumentException("Quantidade não pode ser menor que zero");
            }

            quantidade += Qtd;
        }

        public void RetirarQtd(int Qtd)
        {
            if (Qtd < 0) throw new ArgumentException("Quantidade não pode ser menor que zero");
            else if (Qtd > 0) throw new ArgumentException("A quantidade não pode ser maior que o valor de estoque");

            quantidade -= Qtd;
        }
    }
}
