using Loja.Tabelas.TabelasDto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.TabelasContext
{
    [Table(nameof(HistoricoPagamentos))]
    public class HistoricoPagamentos
    {
        [Key]
        public int Id { get; set; }
        public DateTime Horario { get; private set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; private set; }
        public void SetValor(decimal valor)
        {
            ValorTotal += valor;
        }
        public string StatusPagamento { get; private set; } = "Pendente";
        public void MudarStatus()
        {
            if (StatusPagamento == "Pendente")
            {
                StatusPagamento = "Pago";
            }
            else
            {
                StatusPagamento = "Pendente";
            }
        }
        public string NomeProdutos { get; set; }

        [ForeignKey(nameof(UsuarioRecebedor))]
        public int UserRecebedorId { get; set; }
        public Users UsuarioRecebedor { get; set; }
        [ForeignKey(nameof(UsuarioVendedor))]
        public int UserVendedorId { get; set; }
        public Users UsuarioVendedor { get; set; }
    }
}
