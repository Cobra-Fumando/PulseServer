using Loja.Interfaces;
using Loja.Tabelas;
using Loja.TabelasContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Loja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin loginService;
        private const string Limit = "Fixed";
        public LoginController(ILogin login)
        {
            loginService = login;
        }

        [EnableRateLimiting(Limit)]
        [HttpPost("Logar")]
        public async Task<IActionResult> Logar(UserLogin userLogin)
        {
            var Result = await loginService.Login(userLogin);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem, Token = Result.Dados });
            return Ok(new { Mensagem = Result.Mensagem, Token = Result.Dados });
        }

        [EnableRateLimiting(Limit)]
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Cadastrar(Users users)
        {
            var Result = await loginService.Cadastrar(users);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem });
        }
    }
}