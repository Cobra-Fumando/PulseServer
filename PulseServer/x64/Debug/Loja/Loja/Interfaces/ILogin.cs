using Loja.TabelasContext;
using Loja.Tabelas;

namespace Loja.Interfaces
{
    public interface ILogin
    {
        Task<TabelaProblem<string>> Login(UserLogin userLogin);

        Task<TabelaProblem<UserCadastro>> Cadastrar(Users users);
    }
}
