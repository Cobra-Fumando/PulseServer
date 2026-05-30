using Loja.Conexao;
using Loja.Config;
using Loja.Interfaces;
using Loja.Tabelas;
using Loja.TabelasContext;
using Microsoft.EntityFrameworkCore;

namespace Loja.Components
{
    public class LoginClass : ILogin
    {
        private readonly AppDbContext context;
        public LoginClass(AppDbContext context)
        {
            this.context = context;

        }

        public async Task<TabelaProblem<Users>> Login(UserLogin userLogin)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(userLogin.email) || string.IsNullOrWhiteSpace(userLogin.senha))
                {
                    return StatusProblem.Fail<Users>("Email e senha são obrigatórios.");
                }

                userLogin.email = userLogin.email.Trim().ToLower();

                userLogin.senha = userLogin.senha.Trim().ToLower();

                var user = await context.Users.FirstOrDefaultAsync(us => us.email == userLogin.email);
                if (user == null)
                {
                    return StatusProblem.Fail<Users>("Usuário não encontrado.");
                }

                if (user.senha != userLogin.senha)
                {
                    return StatusProblem.Fail<Users>("Senha ou Email incorreto");
                }

                return StatusProblem.Ok("Login realizado com sucesso.", user);

            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<Users>($"Ocorreu um erro: {ex.Message}");
            }
        }

    }
}
