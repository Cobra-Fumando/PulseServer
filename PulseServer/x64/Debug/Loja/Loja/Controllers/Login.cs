using Loja.Interfaces;
using Loja.Tabelas;
using Microsoft.AspNetCore.Mvc;

namespace Loja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private readonly ILogin loginService;
        public Login(ILogin login)
        {
            loginService = login;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserLogin userLogin)
        {
            var Result = await loginService.Login(userLogin);
            if(!Result.Sucesso) return BadRequest(new {Mensagem = Result.Mensagem});
            return Ok(new { Mensagem = Result.Mensagem });
        }
    }
}