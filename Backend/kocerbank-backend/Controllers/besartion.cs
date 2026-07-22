using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrnekController : ControllerBase
    {
        [HttpGet("topla")]
        public IActionResult Topla()
        {
            int sonuc = 5 + 10;

            return Ok(sonuc);
        }
    }
}