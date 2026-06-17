using Loja.Interfaces;
using Loja.TabelasContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Generic;

namespace Loja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutosConfig lojaConfig;
        private const string Limit = "Fixed";
        public ProdutosController(IProdutosConfig lojaConfig)
        {
            this.lojaConfig = lojaConfig;
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpGet("GetProdutos")]
        public async Task<IActionResult> GetProdutos(int page = 1, int TamanhoPage = 10, bool Ordem = true)
        {
            var Result = await lojaConfig.GetProdutos(page, TamanhoPage, Ordem);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpGet("GetProdutoById/{id}")]
        public async Task<IActionResult> GetProdutoById(int id)
        {
            var Result = await lojaConfig.GetProdutoById(id);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpPost("AddProduto")]
        public async Task<IActionResult> AddProduto(Produtos produto)
        {
            var Result = await lojaConfig.AddProduto(produto);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpGet("SearchProduct")]
        public async Task<IActionResult> SearchProduct(string Name)
        {
            var Result = await lojaConfig.SearchProductByName(Name);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpPost("EnviarEmail")]
        public async Task<IActionResult> EnviarEmail()
        {
            var Result = await lojaConfig.EnviarEmail();
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }
    }
}
