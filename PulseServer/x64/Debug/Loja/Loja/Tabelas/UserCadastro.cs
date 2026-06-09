using System.ComponentModel.DataAnnotations;

namespace Loja.Tabelas
{
    public class UserCadastro
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public required string nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        public required string email { get; set; }
    }
}
