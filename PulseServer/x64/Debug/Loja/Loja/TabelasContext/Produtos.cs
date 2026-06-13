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
    }
}
