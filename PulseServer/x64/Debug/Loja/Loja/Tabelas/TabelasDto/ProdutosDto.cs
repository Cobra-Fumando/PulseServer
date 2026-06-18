using Loja.TabelasContext;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.Tabelas.TabelasDto
{
    public class ProdutosDto
    {
        [Required(ErrorMessage = "O Nome do produto é obrigatório")]
        public required string nome { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public required decimal preco { get; set; }
        public int? Quantidade { get; set; } = 0;
    }
}
