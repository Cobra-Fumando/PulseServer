using Loja.TabelasContext;
using Microsoft.EntityFrameworkCore;

namespace Loja.Conexao
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
         public DbSet<Users> Users { get; set; }
    }
}
