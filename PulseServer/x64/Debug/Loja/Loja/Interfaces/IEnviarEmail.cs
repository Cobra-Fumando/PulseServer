using Loja.Tabelas;

namespace Loja.Interfaces
{
    public interface IEnviarEmail
    {
        Task EnviarEmailAsync(EmailInformation emailInformation);
    }
}
