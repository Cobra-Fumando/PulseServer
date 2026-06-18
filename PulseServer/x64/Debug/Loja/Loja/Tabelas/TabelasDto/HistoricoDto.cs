using Loja.TabelasContext;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.Tabelas.TabelasDto
{
    public class HistoricoDto
    {
        public string? NomeProdutos { get; set; }
        public int Quantidade { get; set; }
        public DateTime Horario { get; set; } = DateTime.UtcNow;
        public UsuarioDto? Usuario { get; set; }
    }
}
