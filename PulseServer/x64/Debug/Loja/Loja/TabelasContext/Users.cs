using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loja.TabelasContext
{
    [Table("Usuarios")]
    [Index(nameof(email), IsUnique = true)]
    public class Users
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório")]
        public required string nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        public required string email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public required string senha { get; set; }
    }
}
