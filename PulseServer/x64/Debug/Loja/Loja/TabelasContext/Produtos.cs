using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.TabelasContext
{
    [Table("Produtos")]
    public class Produtos
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "O Nome do produto é obrigatório")]
        public required string nome { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public required decimal preco { get; set; }
        public string? descricao { get; set; } = null;
        public string? imagemUrl { get; set; } = null;
        public int? Quantidade { get; private set; }
        public void AdicionarQtd(int Qtd)
        {
            if(Qtd <= 0)
            {
                throw new ArgumentException("A quantidade tem que ser maior que zero");
            }

            Quantidade += Qtd;
        }

        public void RetirarQtd(int Qtd)
        {
            if(Qtd <= 0)
            {
                throw new ArgumentException("A quantidade tem que ser maior que zero");
            }else if(Qtd > Quantidade)
            {
                throw new ArgumentException("A quantidade retirada não pode superar o estoque total");
            }

            Quantidade -= Qtd;
        }

        [ForeignKey("Usuario")]
        public int VendedorId { get; set; }
        public Users Usuario { get; set; }
    }
}
