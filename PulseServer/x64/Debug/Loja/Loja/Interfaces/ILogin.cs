using Loja.TabelasContext;
using Loja.Tabelas;

namespace Loja.Interfaces
{
    public interface ILogin
    {
        Task<TabelaProblem<Users>> Login(UserLogin userLogin);
    }
}
