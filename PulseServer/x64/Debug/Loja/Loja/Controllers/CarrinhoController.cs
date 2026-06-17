using Loja.Interfaces;
using Loja.Tabelas;
using Loja.Tabelas.TabelasDto;
using Loja.TabelasContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Loja.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoConfig lojaConfig;
        private const string Limit = "Fixed";
        public CarrinhoController(ICarrinhoConfig lojaConfig)
        {
            this.lojaConfig = lojaConfig;
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpGet("GetCarrinho")]
        public async Task<IActionResult> GetCarrinho()
        {
            var Result = await lojaConfig.GetCarrinho();
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            decimal? valortotal = 0;
            foreach (CarrinhoDto carrinhoDto in Result.Dados!)
            {
                valortotal += (carrinhoDto.Produto.preco * carrinhoDto.quantidade);
            }

            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados, ValorTotal = valortotal });
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpPost("AddCarrinho")]
        public async Task<IActionResult> AddCarrinho(CarrinhoParam carrinho)
        {
            var Result = await lojaConfig.AddCarrinho(carrinho);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }

        [EnableRateLimiting(Limit)]
        [Authorize]
        [HttpDelete("RemoveCarrinho/{carrinhoId}")]
        public async Task<IActionResult> RemoveCarrinho(int carrinhoId)
        {
            var Result = await lojaConfig.RemoveCarrinho(carrinhoId);
            if (!Result.Sucesso) return BadRequest(new { Mensagem = Result.Mensagem });
            return Ok(new { Mensagem = Result.Mensagem, Dados = Result.Dados });
        }

    }
}
