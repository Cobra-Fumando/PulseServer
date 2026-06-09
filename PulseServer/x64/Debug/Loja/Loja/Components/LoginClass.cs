using Loja.Classes;
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
        private readonly Token token;
        private readonly Hash hash;
        public LoginClass(AppDbContext context, Token token, Hash hash)
        {
            this.context = context;
            this.token = token;
            this.hash = hash;
        }

        public async Task<TabelaProblem<string>> Login(UserLogin userLogin)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(userLogin.email) || string.IsNullOrWhiteSpace(userLogin.senha))
                {
                    return StatusProblem.Fail<string>("Email e senha são obrigatórios.");
                }

                userLogin.email = userLogin.email.Trim();
                userLogin.senha = userLogin.senha.Trim();

                var user = await context.Users.FirstOrDefaultAsync(us => us.email == userLogin.email);
                if (user == null)
                {
                    return StatusProblem.Fail<string>("Senha ou Email incorreto.");
                }

                bool verificado = hash.verificarHash(userLogin.senha, user.senha);
                if (!verificado)
                {
                    return StatusProblem.Fail<string>("Senha ou Email incorreto.");
                }

                string TokenGerado = token.GenerateToken(user.nome, user.id);
                if (string.IsNullOrWhiteSpace(TokenGerado))
                {
                    return StatusProblem.Fail<string>("Ocorreu um erro ao gerar o token.");
                }

                return StatusProblem.Ok<string>("Login realizado com sucesso.", TokenGerado);

            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<string>($"Ocorreu um erro: {ex.Message}");
            }
        }

        public async Task<TabelaProblem<UserCadastro>> Cadastrar(Users users)
        {
            try
            {

                users.email = users.email.Trim();
                users.senha = hash.GerarCryp(users.senha);

                await context.Users.AddAsync(users);
                await context.SaveChangesAsync();

                return StatusProblem.Ok<UserCadastro>("Cadastro realizado com sucesso.", new UserCadastro { nome = users.nome, email = users.email });
            }
            catch (DbUpdateException)
            {
                return StatusProblem.Fail<UserCadastro>("Já existe esse email cadastrado");
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<UserCadastro>($"Ocorreu um erro: {ex.Message}");
            }
        }

    }
}
