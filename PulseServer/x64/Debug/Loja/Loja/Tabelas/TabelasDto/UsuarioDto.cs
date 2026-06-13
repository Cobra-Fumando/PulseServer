using System.ComponentModel.DataAnnotations;

namespace Loja.Tabelas.TabelasDto
{
    public class UsuarioDto
    {
        public required string nome { get; set; }
        public required string email { get; set; }

    }
}
