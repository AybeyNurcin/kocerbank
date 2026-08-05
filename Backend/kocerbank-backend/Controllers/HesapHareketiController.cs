using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HesapHareketiController : ControllerBase
    {
        private readonly HesapHareketiService _hesapHareketiService;

        public HesapHareketiController(HesapHareketiService hesapHareketiService)
        {
            _hesapHareketiService = hesapHareketiService;
        }

        [HttpPost("Listele/{hesapBilgileriId:long}")]
        public IActionResult Listele(long hesapBilgileriId)
        {
            try
            {
                List<HesapHareketiDTO> hesapHareketleri =
                    _hesapHareketiService.Listele(hesapBilgileriId);

                return Ok(hesapHareketleri);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }
    }
}