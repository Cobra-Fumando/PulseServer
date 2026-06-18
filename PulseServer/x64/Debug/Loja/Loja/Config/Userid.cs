using Microsoft.AspNetCore.Http;

namespace Loja.Config
{
    public class Userid
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public Userid(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var tokenId = httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(tokenId, out int id) ? id : throw new ArgumentException("Nenhum id encontrado");
        }
    }
}
