using System.ComponentModel.DataAnnotations;

namespace Loja.Tabelas
{
    public class UserLogin
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        public required string email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public required string senha { get; set; }
    }
}
